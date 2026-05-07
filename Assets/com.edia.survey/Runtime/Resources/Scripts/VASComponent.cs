using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {

    public class VASComponent : SurveyComponent {
        [Header("Settings")]
        public Edia.Survey.Constants.VasTypes Type = Edia.Survey.Constants.VasTypes.Slider;

        public Edia.Survey.Constants.IndicatorTypes IndicatorType = Edia.Survey.Constants.IndicatorTypes.Lines;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _topiclabel;

        [SerializeField] private TextMeshProUGUI _topicQuestion;
        [SerializeField] private TextMeshProUGUI inputLabelLeft;
        [SerializeField] private TextMeshProUGUI inputLabelRight;
        [SerializeField] private Slider sliderObj;
        [SerializeField] private RectTransform _markersContainer;
        [SerializeField] private RectTransform _indicatorsContainer;
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private GameObject markerPrefab;

        private readonly List<GameObject> _spawnedItems = new List<GameObject>();

        public override void Init(ComponentData data) {
            base.Init(data);

            switch (data.type) {
                case "VAS_Markers": Type = Constants.VasTypes.Markers; break;
                case "VAS_Slider": Type = Constants.VasTypes.Slider; break;
            }

            if (!string.IsNullOrEmpty(data.indicatorType)) {
                if (System.Enum.TryParse(data.indicatorType, true, out Constants.IndicatorTypes indicatorEnum)) {
                    IndicatorType = indicatorEnum;
                }
            }

            _topiclabel.text = data.label + ":";
            _topicQuestion.text = data.text;
            inputLabelLeft.text = data.leftLabel;
            inputLabelRight.text = data.rightLabel;

            float min = 0;
            float max = 0;
            bool forceFloat = false;

            if (float.TryParse(data.minValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float minVal)) {
                min = minVal;
                if (data.minValue.Contains(".") || data.minValue.Contains(",")) forceFloat = true;
            }

            if (float.TryParse(data.maxValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float maxVal)) {
                max = maxVal;
                if (data.maxValue.Contains(".") || data.maxValue.Contains(",")) forceFloat = true;
            }

            GenerateItems(min, max, forceFloat);
        }

        private void GenerateItems(float min, float max, bool forceFloat = false) {
            if (indicatorPrefab == null || _indicatorsContainer == null) return;

            // Cleanup old items
            foreach (Transform child in _indicatorsContainer) {
                Destroy(child.gameObject);
            }
            if (_markersContainer != null) {
                foreach (Transform child in _markersContainer) {
                    Destroy(child.gameObject);
                }
            }

            _spawnedItems.Clear();

            // Slider visibility and basic setup
            if (Type == Constants.VasTypes.Slider) {
                sliderObj.gameObject.SetActive(true);
                sliderObj.minValue = min;
                sliderObj.maxValue = max;
                bool isWhole = Mathf.Approximately(min, Mathf.Round(min)) && Mathf.Approximately(max, Mathf.Round(max));
                sliderObj.wholeNumbers = isWhole && !forceFloat;
                sliderObj.value = (max + min) / 2f;
            }
            else {
                sliderObj.gameObject.SetActive(false);
            }

            var count = Mathf.FloorToInt(max - min);

            // Generate Indicators if type is not None
            if (IndicatorType != Constants.IndicatorTypes.None) {
                for (int i = 0; i <= count; i++) {
                    GameObject item = Instantiate(indicatorPrefab, _indicatorsContainer);
                    item.name = "Indicator_" + (min + i).ToString();

                    var indicator = item.GetComponent<VASIndicator>();
                    if (indicator != null) indicator.SetIndicator(IndicatorType, i, (min + i).ToString());
                }
            }

            // Generate Markers if type is Markers
            if (Type == Constants.VasTypes.Markers && markerPrefab != null && _markersContainer != null) {
                // Ensure we have a ToggleGroup so only one marker can be active
                var group = _markersContainer.GetComponent<ToggleGroup>();
                if (group == null) group = _markersContainer.gameObject.AddComponent<ToggleGroup>();
                group.allowSwitchOff = false;

                for (int i = 0; i <= count; i++) {
                    GameObject item = Instantiate(markerPrefab, _markersContainer);
                    item.name = (min + i).ToString();

                    var toggle = item.GetComponent<Toggle>();
                    if (toggle != null) toggle.group = group;

                    _spawnedItems.Add(item);
                }

                var middleIndex = Mathf.FloorToInt(count / 2);
                if (_spawnedItems.Count > middleIndex) {
                    _spawnedItems[middleIndex].GetComponent<Toggle>().isOn = true;
                }
            }
        }

        public override void SetValue(string value) {
            if (string.IsNullOrEmpty(value) || value == "N.A.") return;

            switch (Type) {
                case Constants.VasTypes.Markers:
                    foreach (var item in _spawnedItems) {
                        var toggle = item.GetComponent<Toggle>();
                        if (toggle != null) {
                            toggle.isOn = item.name == value;
                        }
                    }
                    break;
                case Constants.VasTypes.Slider:
                    if (float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val)) {
                        sliderObj.value = val;
                    }
                    break;
            }
        }

        public override string GetValue() {
            switch (Type) {
                case Constants.VasTypes.Markers:
                    if (_markersContainer == null) return "N.A.";
                    var group = _markersContainer.GetComponent<ToggleGroup>();
                    if (group == null) return "N.A.";
                    var activeToggle = group.ActiveToggles().FirstOrDefault();
                    if (activeToggle != null) return activeToggle.gameObject.name;
                    Debug.LogWarning("No toggle is currently active in the VAS Markers group");
                    return "N.A.";
                case Constants.VasTypes.Slider:
                    return sliderObj.value.ToString();
                default:
                    return "N.A.";
            }
        }
        public override void ResetComponent() {
            Debug.Log("ResetComponent");
        }
    }
}

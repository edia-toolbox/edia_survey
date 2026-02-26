using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {

    public class VASComponent : SurveyComponent {
        [Header("Settings")]
        public Constants.VasTypes type = Constants.VasTypes.Slider;

        [Tooltip("Only applies to Slider type")]
        public Constants.IndicatorTypes indicatorType = Constants.IndicatorTypes.Lines;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI topiclabel;

        [SerializeField] private TextMeshProUGUI topicQuestion;
        [SerializeField] private TextMeshProUGUI inputLabelLeft;
        [SerializeField] private TextMeshProUGUI inputLabelRight;
        [SerializeField] private Slider sliderObj;
        [SerializeField] private RectTransform itemsContainer;
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private GameObject markerPrefab;

        private readonly List<GameObject> _spawnedItems = new List<GameObject>();

        public override void Init(ComponentData data) {
            base.Init(data);

            switch (data.type) {
                case "VAS_Markers": type = Constants.VasTypes.Markers; break;
                case "VAS_Slider": type = Constants.VasTypes.Slider; break;
            }

            topiclabel.text = data.label + ":";
            topicQuestion.text = data.text;
            inputLabelLeft.text = data.leftLabel;
            inputLabelRight.text = data.rightLabel;

            GenerateItems(data.minValue, data.maxValue);
        }

        private void GenerateItems(int min, int max) {
            if (indicatorPrefab == null || itemsContainer == null) return;

            // Cleanup old indicators
            foreach (Transform child in itemsContainer) {
                Destroy(child.gameObject);
            }

            _spawnedItems.Clear();

            if (type == Constants.VasTypes.Markers) {
                // Ensure we have a ToggleGroup so only one indicator can be active
                var group = itemsContainer.GetComponent<ToggleGroup>();
                if (group == null) group = itemsContainer.gameObject.AddComponent<ToggleGroup>();
                group.allowSwitchOff = false; // Prevent all toggles from being off
            }
            else {
                sliderObj.gameObject.SetActive(true);
                sliderObj.minValue = min;
                sliderObj.maxValue = max;
            }

            var count = Mathf.FloorToInt(max - min);

            for (int i = 0; i <= count; i++) {
                GameObject item = Instantiate(type == Constants.VasTypes.Markers ? markerPrefab : indicatorPrefab, itemsContainer);
                item.name = (min + i).ToString();

                switch (type) {
                    case Constants.VasTypes.Markers:
                        item.GetComponent<Toggle>().group = itemsContainer.GetComponent<ToggleGroup>();
                        break;
                    case Constants.VasTypes.Slider:
                        item.GetComponent<VASIndicator>().SetIndicator(indicatorType, i, i.ToString());
                        break;
                }

                _spawnedItems.Add(item);
            }

            var middle = Mathf.FloorToInt(count / 2);

            if (type == Constants.VasTypes.Markers) {
                _spawnedItems[middle].GetComponent<Toggle>().isOn = true;
            }
            else {
                sliderObj.value = middle;
            }
        }

        public override string GetValue() {
            switch (type) {
                case Constants.VasTypes.Markers:
                    var activeToggle = itemsContainer.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
                    if (activeToggle != null) return activeToggle.gameObject.name;
                    Debug.LogWarning("No toggle is currently active in the VAS Markers group");
                    return "N.A.";
                case Constants.VasTypes.Slider:
                    return sliderObj.value.ToString();
                    break;
                default:
                    return "N.A.";
            }
        }

        public override void ResetComponent() {
            Debug.Log("ResetComponent");
        }
    }

}
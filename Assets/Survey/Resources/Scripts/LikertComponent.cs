using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {
    public class LikertComponent : SurveyComponent {

        [Header("References")]
        [SerializeField] private TextMeshProUGUI ComponentLabel;
        [SerializeField] private Transform TogglesContainer;
        [SerializeField] private GameObject TogglePrefab;
        [SerializeField] private Transform LabelsContainer;
        [SerializeField] private GameObject LabelPrefab;

        private List<string> _namingPool = null;
        private readonly List<GameObject> _generatedObjects = new List<GameObject>();
        
        public override void Init(ComponentData data) {
            base.Init(data);
            if (ComponentLabel != null) ComponentLabel.text = data.label;

            // Clear existing
            foreach (var t in _generatedObjects) {
                if (t != null) {
                    t.transform.SetParent(null);
                    Destroy(t);
                }
            }
            _generatedObjects.Clear();
            
            // Also clear containers just in case there are other children
            if (TogglesContainer != null) {
                while (TogglesContainer.childCount > 0) {
                    Transform child = TogglesContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }
            if (LabelsContainer != null) {
                while (LabelsContainer.childCount > 0) {
                    Transform child = LabelsContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }

            if (data.options == null) return;
                
            var count = data.options.Count;
            TogglesContainer.GetComponent<GridLayoutGroup>().constraintCount = count;
            LabelsContainer.GetComponent<GridLayoutGroup>().constraintCount = count;

            int middleIndex = count / 2;

            // For Faces, we need correct naming of the toggles
            switch (count) {
                case 3:
                    _namingPool = new List<string>() { "0", "3", "6" }; break;
                case 5:
                    _namingPool = new List<string>() { "0", "2", "3", "4", "6" }; break;
                case 7:
                    _namingPool = new List<string>() { "0", "1", "2", "3", "4", "5", "6" }; break;
            }
            
            for (int i = 0; i < data.options.Count; i++) {
                var holder = new GameObject($"{data.options[i]}", typeof(RectTransform));
                holder.transform.SetParent(TogglesContainer);
                holder.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                holder.transform.localScale = Vector3.one;
                
                var go     = Instantiate(TogglePrefab, holder.transform);
                go.name = $"{ (_namingPool == null ? data.options[i] : _namingPool[i])}";
                var toggle = go.GetComponent<Toggle>();
                if (toggle != null) {
                    toggle.group = TogglesContainer.GetComponent<ToggleGroup>();
                    toggle.isOn = (i == middleIndex);
                    _generatedObjects.Add(go);
                } else {
                    Debug.LogWarning($"TogglePrefab does not have a Toggle component attached: {TogglePrefab.name}");
                }
                
                var tLabel = Instantiate(LabelPrefab, LabelsContainer);
                if (tLabel != null) tLabel.GetComponent<TextMeshProUGUI>().text = data.options[i];
                _generatedObjects.Add(tLabel);
            }
        }

        public override string GetValue() {
            var activeToggle = TogglesContainer.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
            if (activeToggle == null) return null;
            
            int index = activeToggle.transform.parent.GetSiblingIndex();
            if (Data.options != null && index >= 0 && index < Data.options.Count) {
                return Data.options[index];
            }

            return activeToggle.name;
        }
    }
}

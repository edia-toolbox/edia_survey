using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {

    /// <summary>
    /// Helper class for a single row in the Matrix Likert Scale.
    /// This should be attached to the rowPrefab.
    /// </summary>
    public class LikertMatrixRow : MonoBehaviour {

        [Header( "References")]
        [SerializeField] private TextMeshProUGUI ComponentLabel;
        [SerializeField] private Transform TogglesContainer;
        [SerializeField] private GameObject TogglePrefab;
        
        public string Topic { get; private set; }
        private List<string> _options;
        private ToggleGroup _toggleGroup;
        private GridLayoutGroup _gridLayoutOptions;

        public void Init(string topic, List<string> options) {
            Topic = topic;
            _options = options;
            if (ComponentLabel != null) ComponentLabel.text = topic;

            // Clear existing
            if (TogglesContainer != null) {
                while (TogglesContainer.childCount > 0) {
                    Transform child = TogglesContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }

            _toggleGroup = TogglesContainer.GetComponent<ToggleGroup>();
            TogglesContainer.GetComponent<GridLayoutGroup>().constraintCount = options.Count;
            
            int middleIndex = options.Count / 2;
            for (int i = 0; i < options.Count; i++) {
                var option = options[i];
                var holder = new GameObject($"{option}Holder", typeof(RectTransform));
                holder.transform.SetParent(TogglesContainer);
                holder.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                holder.transform.localScale = Vector3.one;
                
                var go     = Instantiate(TogglePrefab, holder.transform);
                var toggle = go.GetComponent<Toggle>();
                if (toggle != null) {
                    toggle.group = _toggleGroup;
                    toggle.isOn = (i == middleIndex);
                }
            }
        }

        public string GetValue() {
            if (_toggleGroup == null) return null;
            var activeToggle = _toggleGroup.ActiveToggles().FirstOrDefault();
            if (activeToggle == null) return null;

            int index = activeToggle.transform.parent.GetSiblingIndex();
            if (_options != null && index >= 0 && index < _options.Count) {
                return _options[index];
            }

            return activeToggle.name;
        }
    }

    

}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {
    /// <summary>
    /// A component for a question with multiple possible answers.
    /// Can be configured to allow single or multiple selections.
    /// </summary>
    public class QuestionAnswerComponent : SurveyComponent {
        [SerializeField] private TextMeshProUGUI questionLabel;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private GameObject togglePrefab;
        [SerializeField] private Transform optionsContainer;

        private List<Toggle> _toggles = new List<Toggle>();

        public override void Init(ComponentData data) {
            base.Init(data);
            if (questionLabel != null) questionLabel.text = data.label;

            // Clear existing options
            foreach (var t in _toggles) {
                if (t != null) {
                    t.transform.SetParent(null);
                    Destroy(t.gameObject);
                }
            }
            _toggles.Clear();
            
            if (optionsContainer != null) {
                while (optionsContainer.childCount > 0) {
                    Transform child = optionsContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }

            if (data.options == null) return;

            // Configure ToggleGroup
            if (toggleGroup != null) {
                toggleGroup.allowSwitchOff = true;
            }

            foreach (var option in data.options) {
                var go = Instantiate(togglePrefab, optionsContainer);
                var toggle = go.transform.GetChild(0).GetComponent<Toggle>();
                
                if (toggle != null) {
                    toggle.name = option;
                    // If not allowing multiple, assign to ToggleGroup to enforce single choice
                    if (toggleGroup != null && !data.allowMultiple) {
                        toggle.group = toggleGroup;
                    } else {
                        toggle.group = null;
                    }
                    
                    toggle.isOn = false;
                    _toggles.Add(toggle);
                } else {
                    Debug.LogWarning($"TogglePrefab does not have a Toggle component attached: {togglePrefab.name}");
                }
                
                var tLabel = go.GetComponentInChildren<TextMeshProUGUI>();
                if (tLabel != null) tLabel.text = option;
            }
        }

        public override string GetValue() {
            var selectedOptions = _toggles
                .Where(t => t.isOn)
                .Select(t => t.name)
                .Where(text => !string.IsNullOrEmpty(text))
                .ToList();

            if (selectedOptions.Count == 0) return null;
            
            // Return comma-separated values
            return string.Join(", ", selectedOptions);
        }
    }
}

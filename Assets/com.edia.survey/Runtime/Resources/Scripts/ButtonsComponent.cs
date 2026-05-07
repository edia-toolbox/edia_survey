using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Edia.Survey {
    public class ButtonsComponent : SurveyComponent {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI pageInfoField;

        public Action<string> OnButtonClicked;

        public override void Init(ComponentData data) {
            base.Init(data);
            ClearButtons();
            
            // For manual buttons defined in JSON
            if (!string.IsNullOrEmpty(data.label)) {
                AddButton(data.label, data.buttonAction);
            }
        }

        public void SetPageInfo(int currentPage, int totalPages) {
            if (pageInfoField != null) {
                pageInfoField.text = $"{currentPage + 1} of {totalPages}";
            }
        }

        public void AddButton(string labelText, string action) {
            if (buttonPrefab == null || container == null) {
                Debug.LogWarning($"ButtonComponent on {gameObject.name} is missing buttonPrefab or container reference.");
                return;
            }

            GameObject go = Instantiate(buttonPrefab, container);
            Button btn = go.GetComponent<Button>();
            if (btn == null) btn = go.GetComponentInChildren<Button>();

            if (btn != null) {
                TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) {
                    txt.text = labelText;
                }
                
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnButtonClicked?.Invoke(action));
            } else {
                Debug.LogError($"The buttonPrefab on {gameObject.name} does not have a Button component.");
            }
        }

        public void ClearButtons() {
            if (container == null) return;
            
            while (container.childCount > 0) {
                Transform child = container.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        public override void SetValue(string value) {
            // Buttons don't have a value to restore
        }

        public override string GetValue() {
            return null;
        }
    }
}

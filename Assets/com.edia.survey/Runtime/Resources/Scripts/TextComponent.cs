using TMPro;
using UnityEngine;

namespace Edia.Survey {
    public class TextComponent : SurveyComponent {
        [SerializeField] private TextMeshProUGUI textField;

        public override void Init(ComponentData data) {
            base.Init(data);
            if (textField != null) {
                textField.text = !string.IsNullOrEmpty(data.text) ? data.text : data.label;
            }
        }

        public override void SetValue(string value) {
            // Text blocks don't have a value to restore
        }

        public override string GetValue() {
            return null; // Text blocks don't have a value to collect
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Edia.Survey {
    public class QuestionDropdownAnswerComponent : SurveyComponent {
        [SerializeField] private TextMeshProUGUI questionLabel;
        [SerializeField] private TMP_Dropdown dropdown;

        public override void Init(ComponentData data) {
            base.Init(data);
            
            if (questionLabel != null) {
                questionLabel.text = data.label;
            }

            if (dropdown != null) {
                dropdown.ClearOptions();
                if (data.options != null && data.options.Count > 0) {
                    dropdown.AddOptions(data.options);
                }
            }
        }

        public override string GetValue() {
            if (dropdown != null && dropdown.options.Count > 0) {
                return dropdown.options[dropdown.value].text;
            }
            return "N.A.";
        }

        public override void ResetComponent() {
            if (dropdown != null) {
                dropdown.value = 0;
            }
        }
    }
}

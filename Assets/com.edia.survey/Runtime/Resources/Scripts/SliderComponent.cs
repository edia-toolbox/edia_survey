using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Edia.Survey {
    public class SliderComponent : SurveyComponent {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI labelField;
        [SerializeField] private TextMeshProUGUI subTextField;

        public override void Init(ComponentData data) {
            base.Init(data);
            if (slider != null) {
                slider.minValue = data.minValue;
                slider.maxValue = data.maxValue;
                slider.wholeNumbers = true; // Defaulting to whole numbers for Likert scale
                slider.value = (data.minValue + data.maxValue) / 2f;
            }

            if (labelField != null) {
                labelField.text = data.label;
            }

            if (subTextField != null) {
                subTextField.text = data.text;
            }
        }

        public override string GetValue() {
            return slider != null ? slider.value.ToString() : "0";
        }
    }
}

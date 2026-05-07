using TMPro;
using UnityEngine;

namespace Edia.Survey {
    /// <summary>
    /// Component that displays a 'Thank You' message at the end of the survey.
    /// This component can be placed on a final survey page to inform the user that their submission was successful.
    /// </summary>
    public class SurveyFinishedComponent : SurveyComponent {
        [SerializeField] private TextComponent infoTextComponent;
        [SerializeField] private TextComponent headerTextComponent;
        
        public override void Init(ComponentData data) {
            base.Init(data);
            
            if (infoTextComponent != null) {
                infoTextComponent.Init(data);
            }

            ComponentData tmp = data;
            data.text = data.label; // We want the label to be the text for this header testcomponent
            
            if (headerTextComponent != null) {
                headerTextComponent.Init(tmp);
            }
        }

        public override void SetValue(string value) {
            // No value to restore
        }

        public override string GetValue() {
            return null; // No value to collect
        }
    }
}

using UnityEngine;

namespace Edia.Survey {
    public abstract class SurveyComponent : MonoBehaviour {
        public ComponentData Data { get; private set; }

        public virtual void Init(ComponentData data) {
            Data = data;
        }

        /// <summary>
        /// Returns the current value of the component as a string.
        /// If the component has no value (like a text block), it can return null or empty string.
        /// </summary>
        public abstract string GetValue();
        
        /// <summary>
        /// Sets the current value of the component.
        /// </summary>
        public abstract void SetValue(string value);

        /// <summary>
        /// Resets the component to its default state if needed.
        /// </summary>
        public virtual void ResetComponent() { }
    }
}

using UnityEditor;
using UnityEngine;

namespace Edia.Survey {

    [CustomPropertyDrawer(typeof(Constants.UnitSymbols))]
    public class CurrencySymbolDrawer : PropertyDrawer {
        private readonly string[] enumNames = { "%", "$", "�" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.Enum) {
                property.enumValueIndex = EditorGUI.Popup(position, label.text, property.enumValueIndex, enumNames);
            }
            else {
                EditorGUI.LabelField(position, label.text, "Use only with CurrencySymbol enum.");
            }

            EditorGUI.EndProperty();
        }
    }
}
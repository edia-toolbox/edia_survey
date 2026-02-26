using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {
    [CustomPropertyDrawer(typeof(SurveyComponentTypeAttribute))]
    public class SurveyComponentTypeDrawer : PropertyDrawer {
        private string[] _types;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (_types == null) {
                _types = GetTypes();
            }

            if (_types == null || _types.Length == 0) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            int currentIndex = -1;
            for (int i = 0; i < _types.Length; i++) {
                if (_types[i] == property.stringValue) {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1) {
                // If the current value is not in the list, we show it at the end of the popup
                // or just default to the first one. Let's add it to the list for this drawer instance.
                var tempList = new List<string>(_types);
                tempList.Add(property.stringValue + " (Custom)");
                int newIndex = EditorGUI.Popup(position, label.text, tempList.Count - 1, tempList.ToArray());
                if (newIndex >= 0 && newIndex < tempList.Count - 1) {
                    property.stringValue = _types[newIndex];
                }
            } else {
                int newIndex = EditorGUI.Popup(position, label.text, currentIndex, _types);
                if (newIndex >= 0 && newIndex < _types.Length) {
                    property.stringValue = _types[newIndex];
                }
            }
        }

        private string[] GetTypes() {
            var guids = AssetDatabase.FindAssets("t:SurveyPrefabSettings");
            HashSet<string> types = new HashSet<string>();
            
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var settings = AssetDatabase.LoadAssetAtPath<SurveyPrefabSettings>(path);
                if (settings != null && settings.PrefabMappings != null) {
                    foreach (var mapping in settings.PrefabMappings) {
                        if (!string.IsNullOrEmpty(mapping.type)) {
                            types.Add(mapping.type);
                        }
                    }
                }
            }

            return types.OrderBy(t => t).ToArray();
        }
    }
}

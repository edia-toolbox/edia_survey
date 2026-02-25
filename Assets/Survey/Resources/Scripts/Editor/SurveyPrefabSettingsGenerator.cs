using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Edia.Survey {
    public static class SurveyPrefabSettingsGenerator {
        [MenuItem("EDIA/Survey/Generate Dummy Prefab Settings")]
        public static void GenerateDummySettings() {
            string path = "Assets/EdiaSamples/Survey/SurveyPrefabSettings_Dummy.asset";
            
            SurveyPrefabSettings settings = ScriptableObject.CreateInstance<SurveyPrefabSettings>();
            
            settings.PrefabMappings = new List<SurveyPrefabSettings.ComponentPrefabMap> {
                new SurveyPrefabSettings.ComponentPrefabMap { type = "Header", prefab = null },
                new SurveyPrefabSettings.ComponentPrefabMap { type = "Likert", prefab = null },
                new SurveyPrefabSettings.ComponentPrefabMap { type = "VAS", prefab = null },
                new SurveyPrefabSettings.ComponentPrefabMap { type = "Buttons", prefab = null }
            };

            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Dummy SurveyPrefabSettings created at {path}. Note: Prefabs are null and must be assigned manually.");
            Selection.activeObject = settings;
        }
    }
}

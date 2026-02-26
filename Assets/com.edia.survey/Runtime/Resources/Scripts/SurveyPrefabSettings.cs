using UnityEngine;
using System;
using System.Collections.Generic;

namespace Edia.Survey {
    
    public class SurveyPrefabSettings : ScriptableObject {
        [Serializable]
        public struct ComponentPrefabMap {
            public string type;
            public GameObject prefab;
        }

        public List<ComponentPrefabMap> PrefabMappings = new List<ComponentPrefabMap>();

        public GameObject GetPrefabForType(string type) {
            if (PrefabMappings == null) return null;
            
            foreach (var mapping in PrefabMappings) {
                if (string.Equals(mapping.type, type, StringComparison.OrdinalIgnoreCase)) {
                    return mapping.prefab;
                }
            }
            return null;
        }
    }
}

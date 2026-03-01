using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Survey {
    public class SurveyComponentTypeAttribute : PropertyAttribute { }

    [Serializable]
    public class ComponentData {
        [Tooltip("Unique ID for this entry to track results (Used by: All)")]
        public string id; // Unique ID for this entry to track results
        [SurveyComponentType]
        [Tooltip("The component type (Used by: All)")]
        public string type;
        [Tooltip("Main content text (Used by: Header, Text, Slider, VAS)")]
        public string text;
        [Tooltip("Header label for the component (Used by: Buttons, Likert, LikertMatrix, QuestionAnswer, QuestionDropdownAnswer, Slider, VAS)")]
        public string label;
        [Tooltip("List of selection options (Used by: Likert, LikertMatrix, QuestionAnswer, QuestionDropdownAnswer)")]
        public List<string> options;
        [Tooltip("Minimum value for range-based components (Used by: Slider, VAS)")]
        public string minValue;
        [Tooltip("Maximum value for range-based components (Used by: Slider, VAS)")]
        public string maxValue;
        [Tooltip("Label displayed on the left side (Used by: VAS)")]
        public string leftLabel; // For Semantic Differential or VAS
        [Tooltip("Label displayed on the right side (Used by: VAS)")]
        public string rightLabel; // For Semantic Differential or VAS
        [Tooltip("Action triggered when clicking the button (Used by: Buttons)")]
        public string buttonAction;
        [Tooltip("Allow multiple selections if true, otherwise single selection (Used by: QuestionAnswer)")]
        public bool allowMultiple; // For QuestionAnswer component
        [Tooltip("List of questions for matrix components (Used by: LikertMatrix)")]
        public List<string> subQuestions; // For Matrix Likert component
        [Tooltip("Type of indicator to display (Used by: VAS)")]
        public string indicatorType;
    }

    [Serializable]
    public class PageData {
        public string id;
        public List<ComponentData> components;
    }

    [Serializable]
    public class SurveyData {
        public string surveyId;
        public List<PageData> pages;
    }

    [Serializable]
    public class SurveyResult {
        public string SurveyId;
        public List<ComponentResult> results = new List<ComponentResult>();

        /// <summary>
        /// Parses a JSON string back into a SurveyResult object.
        /// </summary>
        public static SurveyResult Parse(string json) {
            return JsonUtility.FromJson<SurveyResult>(json);
        }

        /// <summary>
        /// Converts the results list to a Dictionary with componentId as the key.
        /// </summary>
        public Dictionary<string, string> ToDictionary() {
            var dict = new Dictionary<string, string>();
            foreach (var r in results) {
                if (!string.IsNullOrEmpty(r.componentId)) {
                    dict[r.componentId] = r.value;
                }
            }
            return dict;
        }

        /// <summary>
        /// Converts the results list to a List of KeyValuePairs.
        /// </summary>
        public List<KeyValuePair<string, string>> ToKeyValuePairList() {
            var list = new List<KeyValuePair<string, string>>();
            foreach (var r in results) {
                list.Add(new KeyValuePair<string, string>(r.componentId, r.value));
            }
            return list;
        }
    }

    [Serializable]
    public class ComponentResult {
        public string componentId;
        public string value;
    }
}

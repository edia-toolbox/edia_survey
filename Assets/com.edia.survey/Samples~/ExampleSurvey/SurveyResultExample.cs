using UnityEngine;
using Edia.Survey;
using System.Collections.Generic;

namespace Edia.Survey.Examples {
    /// <summary>
    /// This example script demonstrates how to parse the survey results JSON back into a usable format.
    /// You can attach this script to a GameObject and assign the 'OnResultsReceived' method 
    /// to the 'OnSurveyFinishedResult(string)' event in the SurveyManager component.
    /// </summary>
    public class SurveyResultExample : MonoBehaviour {
        
        /// <summary>
        /// This method should be assigned to the OnSurveyFinishedResult(string) event in the SurveyManager inspector.
        /// </summary>
        /// <param name="json">The JSON string containing the survey results.</param>
        public void OnResultsReceived(string json) {
            
            // 1. Parse the JSON back into the SurveyResult data structure.
            // You can use the convenience method added to the SurveyResult class.
            SurveyResult surveyResult = SurveyResult.Parse(json);
            
            Debug.Log($"--- Survey Results Received for ID: {surveyResult.SurveyId} ---");

            // 2. Convert to a List of KeyValuePairs (as requested).
            List<KeyValuePair<string, string>> pairs = surveyResult.ToKeyValuePairList();
            
            foreach (var pair in pairs) {
                Debug.Log($"[Pair] Component: {pair.Key}, Value: {pair.Value}");
            }

            // 3. Alternatively, convert to a Dictionary for easy lookup.
            Dictionary<string, string> resultDict = surveyResult.ToDictionary();

            // Example of how to access a specific component's value by ID if you know it.
            // If you have a component with id "feedback_comment", you could do:
            if (resultDict.TryGetValue("feedback_comment", out string comment)) {
                Debug.Log($"Feedback comment: {comment}");
            }
        }
    }
}

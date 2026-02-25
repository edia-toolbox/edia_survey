using UnityEngine;
using System.Collections.Generic;

namespace Edia.Survey {
    [CreateAssetMenu(fileName = "NewSurvey", menuName = "EDIA/Survey/Survey Configuration")]
    public class SurveyConfiguration : ScriptableObject {
        public SurveyData Data;
    }
}
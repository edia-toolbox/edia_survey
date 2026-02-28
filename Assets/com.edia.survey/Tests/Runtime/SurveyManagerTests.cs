using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Edia.Survey;

namespace Edia.Survey.Tests.Runtime
{
    public class SurveyManagerTests
    {
        private GameObject _surveyManagerGo;
        private SurveyManager _surveyManager;

        [SetUp]
        public void SetUp()
        {
            _surveyManagerGo = new GameObject("SurveyManager");
            _surveyManager = _surveyManagerGo.AddComponent<SurveyManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_surveyManagerGo);
        }

        [UnityTest]
        public IEnumerator SurveyManager_InitWithData_DoesNotCrash()
        {
            // Arrange
            var data = new SurveyData
            {
                surveyId = "test_survey_runtime",
                pages = new List<PageData>
                {
                    new PageData
                    {
                        id = "p1",
                        components = new List<ComponentData>()
                    }
                }
            };

            // Act
            // We expect some warnings because PrefabReferences are null, but it shouldn't crash.
            _surveyManager.Init(data);

            yield return null;

            // Assert
            Assert.Pass("SurveyManager initialized without crashing.");
        }
        
        [Test]
        public void SurveyManager_Exists()
        {
            Assert.IsNotNull(_surveyManager);
        }
    }
}
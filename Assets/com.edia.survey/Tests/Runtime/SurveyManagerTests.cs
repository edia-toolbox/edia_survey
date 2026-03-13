using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Edia.Survey
{
    public class SurveyManagerTests
    {
        private GameObject _surveyManagerGo;
        private SurveyManager _surveyManager;

        [SetUp]
        public void SetUp()
        {
            _surveyManagerGo = new GameObject("SurveyManager");
            _surveyManagerGo.SetActive(false);
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
            _surveyManager.Init(data);
            _surveyManagerGo.SetActive(true);

            yield return null;

            // Assert
            Assert.Pass("SurveyManager initialized without crashing.");
        }
        
        [Test]
        public void SurveyManager_Exists()
        {
            Assert.IsNotNull(_surveyManager);
        }

        [Test]
        public void SurveyManager_SubmitSurvey_OnLastPage_TriggersOnSurveyFinishedResult()
        {
            // Arrange
            bool eventTriggered = false;
            string resultsJson = null;

            _surveyManager.OnSurveyFinishedResult.AddListener((json) => {
                eventTriggered = true;
                resultsJson = json;
            });

            var data = new SurveyData
            {
                surveyId = "test_survey_finish",
                pages = new List<PageData>
                {
                    new PageData
                    {
                        id = "p1",
                        components = new List<ComponentData>()
                    }
                }
            };
            _surveyManager.Init(data);

            // Act
            _surveyManager.SubmitSurvey();

            // Assert
            Assert.IsTrue(eventTriggered, "OnSurveyFinishedResult should be triggered when SubmitSurvey is called on the last page.");
            Assert.IsNotNull(resultsJson, "Results JSON should not be null.");
        }
    }
}
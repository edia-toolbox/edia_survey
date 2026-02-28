using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Edia.Survey;

namespace Edia.Survey.Tests.Editor
{
    public class SurveyDataTests
    {
        [Test]
        public void SurveyData_JsonSerialization_WorksCorrectly()
        {
            // Arrange
            var data = new SurveyData
            {
                surveyId = "test_survey",
                pages = new List<PageData>
                {
                    new PageData
                    {
                        id = "p1",
                        components = new List<ComponentData>
                        {
                            new ComponentData
                            {
                                id = "c1",
                                type = "text",
                                text = "Hello World"
                            }
                        }
                    }
                }
            };

            // Act
            string json = JsonUtility.ToJson(data);
            var deserialized = JsonUtility.FromJson<SurveyData>(json);

            // Assert
            Assert.AreEqual(data.surveyId, deserialized.surveyId);
            Assert.AreEqual(data.pages.Count, deserialized.pages.Count);
            Assert.AreEqual(data.pages[0].id, deserialized.pages[0].id);
            Assert.AreEqual(data.pages[0].components[0].id, deserialized.pages[0].components[0].id);
            Assert.AreEqual(data.pages[0].components[0].text, deserialized.pages[0].components[0].text);
        }

        [Test]
        public void ComponentResult_Serialization_WorksCorrectly()
        {
            // Arrange
            var result = new SurveyResult
            {
                SurveyId = "test_id",
                results = new List<ComponentResult>
                {
                    new ComponentResult { componentId = "q1", value = "ans1" }
                }
            };

            // Act
            string json = JsonUtility.ToJson(result);
            var deserialized = JsonUtility.FromJson<SurveyResult>(json);

            // Assert
            Assert.AreEqual(result.SurveyId, deserialized.SurveyId);
            Assert.AreEqual(result.results.Count, deserialized.results.Count);
            Assert.AreEqual(result.results[0].componentId, deserialized.results[0].componentId);
            Assert.AreEqual(result.results[0].value, deserialized.results[0].value);
        }
    }
}
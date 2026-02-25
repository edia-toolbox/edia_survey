using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;


namespace Edia.Survey {
    [EdiaHeader("EDIA SURVEY", "Modular Toolkit", "Collects user feedback via variety of components")]
    public class SurveyManager : MonoBehaviour {

        [Header("Settings")]
        public SurveyConfiguration SurveyConfig;
        public TextAsset SurveyJson;
        public SurveyPrefabSettings PrefabSettings;
        
        [Header("Events")]
        public UnityEvent onFinished;

        private SurveyData _data;
        private int _currentPageIndex = -1;
        private readonly SurveyResult _results = new SurveyResult();
        private readonly List<SurveyComponent> _activeComponents = new List<SurveyComponent>();

        private void Start() {
            LoadSurvey();
            if (_data != null && _data.pages.Count > 0) {
                ShowPage(0);
            }
        }

        private void LoadSurvey() {
            if (SurveyConfig != null && SurveyConfig.Data != null) {
                _data = SurveyConfig.Data;
                Debug.Log($"Survey loaded from ScriptableObject with {_data.pages.Count} pages.");
            } else if (SurveyJson != null) {
                string json = SurveyJson.text;
                _data = JsonUtility.FromJson<SurveyData>(json);
                if (_data != null) {
                    Debug.Log($"Survey loaded from JSON with {_data.pages.Count} pages.");
                } else {
                    Debug.LogError("Failed to parse Survey JSON.");
                }
            } else {
                Debug.LogError("No Survey Configuration or JSON file assigned in the inspector.");
            }
        }

        private void ShowPage(int index) {
            if (_data == null || index < 0 || index >= _data.pages.Count) {
                Debug.LogWarning("Page index out of range or data not loaded.");
                return;
            }

            // Collect results from current page before clearing
            if (_currentPageIndex != -1) {
                CollectCurrentPageResults();
            }

            ClearPage();

            _currentPageIndex = index;
            PageData page = _data.pages[index];

            foreach (var compData in page.components) {
                InstantiateComponent(compData);
            }

            AddNavigationButtons(index, _data.pages.Count);
            ForceLayoutUpdate();
        }

        private void ForceLayoutUpdate() {
                // Ensure all UI elements have calculated their sizes
                Canvas.ForceUpdateCanvases();
                
                // Force layout rebuild on the container
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                if (rectTransform != null) {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                }
        }

        private void ClearPage() {
            
            while (this.transform.childCount > 0) {
                Transform child = this.transform.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            _activeComponents.Clear();
        }

        private void InstantiateComponent(ComponentData compData) {
            GameObject prefab = GetPrefabForType(compData.type);
            if (prefab != null) {
                GameObject go   = Instantiate(prefab, this.transform);
                var        comp = go.GetComponent<SurveyComponent>();
                if (comp != null) {
                    comp.Init(compData);
                    _activeComponents.Add(comp);

                    // Handle button actions
                    if (comp is ButtonsComponent btn) {
                        btn.OnButtonClicked = HandleButtonAction;
                    }
                }
            } else {
                Debug.LogWarning($"No prefab found for component type: {compData.type}");
            }
        }

        private void AddNavigationButtons(int pageIndex, int totalPages) {
            GameObject prefab = GetPrefabForType("Buttons");
            if (prefab == null) {
                Debug.LogWarning("Cannot add automatic navigation buttons: 'Buttons' prefab mapping not found.");
                return;
            }

            GameObject go      = Instantiate(prefab, this.transform);
            var        btnComp = go.GetComponent<ButtonsComponent>();
            if (btnComp != null) {
                // Initialize with dummy data to satisfy base class and clear any defaults
                btnComp.Init(new ComponentData { type = "Buttons", id = $"nav_page_{pageIndex}" });
                btnComp.OnButtonClicked = HandleButtonAction;
                btnComp.SetPageInfo(pageIndex, totalPages);

                // Previous button on all pages except the first
                if (pageIndex > 0) {
                    btnComp.AddButton("Back", "Back");
                }

                // Next button on all pages except the last, Submit on the last
                if (pageIndex < totalPages - 1) {
                    btnComp.AddButton("Next", "Next");
                } else {
                    btnComp.AddButton("Submit", "Submit");
                }
                
                _activeComponents.Add(btnComp);
            }
        }

        private GameObject GetPrefabForType(string type) {
            return PrefabSettings != null ? PrefabSettings.GetPrefabForType(type) : null;
        }

        private void HandleButtonAction(string action) {
            switch (action.ToLower()) {
                case "nextpage":
                case "next":
                    ShowPage(_currentPageIndex + 1);
                    break;
                case "previouspage":
                case "back":
                    ShowPage(_currentPageIndex - 1);
                    break;
                case "submit":
                    SubmitSurvey();
                    break;
                case "finish":
                    onFinished?.Invoke();
                    break;
                default:
                    Debug.Log($"Action triggered: {action}");
                    break;
            }
        }

        private void CollectCurrentPageResults() {
            foreach (var comp in _activeComponents) {
                string value = comp.GetValue();
                if (value != null && !string.IsNullOrEmpty(comp.Data.id)) {
                    // Update or add result
                    var existing = _results.results.Find(r => r.componentId == comp.Data.id);
                    if (existing != null) {
                        existing.value = value;
                    } else {
                        _results.results.Add(new ComponentResult { componentId = comp.Data.id, value = value });
                    }
                }
            }
        }

        public void SubmitSurvey() {
            CollectCurrentPageResults();
            string jsonResults = JsonUtility.ToJson(_results, true);
            Debug.Log("Survey Submitted:\n" + jsonResults);
            
            // Example: Save to persistent data path
            string savePath = Path.Combine(Application.persistentDataPath, "results.json");
            File.WriteAllText(savePath, jsonResults);
            Debug.Log("Results saved to: " + savePath);

            ShowThankYou();
        }

        private void ShowThankYou() {
            ClearPage();
            _currentPageIndex = -1;

            InstantiateComponent(new ComponentData {
                type = "Header",
                text = "Thank you"
            });

            InstantiateComponent(new ComponentData {
                type = "Buttons",
                label = "Finish",
                buttonAction = "Finish"
            });
            
            ForceLayoutUpdate();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Edia.Survey {

    [EdiaHeader("EDIA SURVEY", "Modular Toolkit", "Collects user feedback via variety of components")]
    public class SurveyManager : MonoBehaviour {
        [Header("Settings")]
        [Tooltip("Either provide a SurveyConfiguration or a JSON string to load the survey data")]
        public TextAsset SurveyJson;

        [Tooltip("Either provide a SurveyConfiguration or a JSON string to load the survey data")]
        public SurveyConfiguration SurveyConfig;

        [Space(20)]
        [Header("Callbacks")]
        [Tooltip("Optional: Triggered when the survey is finished")]
        public UnityEvent OnSurveyFinished;

        [Tooltip("Optional: Triggered when the survey is finished and provides results.json as a string")]
        public UnityEvent<string> OnSurveyFinishedResult;

        [SerializeField, HideInInspector]
        private SurveyPrefabSettings PrefabReferences;

        private SurveyData _data;
        private int _currentPageIndex = -1;
        private readonly SurveyResult _results = new SurveyResult();
        private readonly List<SurveyComponent> _activeComponents = new List<SurveyComponent>();

        public int CurrentPageIndex => _currentPageIndex;
        public SurveyData Data => _data;

        private void Start() {
            EnsureGraphicRaycasterExists();

            if (_data == null) {
                LoadSurvey();
            }

            if (_data != null && _data.pages.Count > 0) {
                ShowPage(0);
            }
        }

        private void EnsureGraphicRaycasterExists() {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null) {
                TrackedDeviceGraphicRaycaster raycaster = canvas.GetComponent<TrackedDeviceGraphicRaycaster>();
                if (raycaster == null) {
                    canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                    Debug.Log("GraphicRaycaster added to Canvas.");
                }
            }
            else {
                Debug.LogWarning("No Canvas found in parent hierarchy.");
            }
        }

        /// <summary>
        /// Initializes the survey using the JSON file or ScriptableObject assigned in the inspector.
        /// </summary>
        [ContextMenu("Initialize Survey")]
        public void Init() {
            LoadSurvey();
            if (_data != null && _data.pages.Count > 0) {
                ShowPage(0);
            }
        }

        /// <summary>
        /// Initializes the survey with a JSON configuration string.
        /// </summary>
        /// <param name="json">The JSON string containing survey data.</param>
        public void Init(string json) {
            _data = JsonUtility.FromJson<SurveyData>(json);
            if (_data != null) {
                _results.SurveyId = _data.surveyId;
                Debug.Log($"Survey initialized from JSON string with {_data.pages.Count} pages.");
                ShowPage(0);
            }
            else {
                Debug.LogError("Failed to parse Survey JSON provided to Init.");
            }
        }

        /// <summary>
        /// Initializes the survey with a SurveyData object.
        /// </summary>
        /// <param name="data">The SurveyData object.</param>
        public void Init(SurveyData data) {
            _data = data;
            if (_data != null) {
                _results.SurveyId = _data.surveyId;
                Debug.Log($"Survey initialized from SurveyData with {_data.pages.Count} pages.");
                ShowPage(0);
            }
            else {
                Debug.LogError("SurveyData provided to Init is null.");
            }
        }

        /// <summary>
        /// Initializes the survey with a SurveyConfiguration ScriptableObject.
        /// </summary>
        /// <param name="config">The SurveyConfiguration ScriptableObject.</param>
        public void Init(SurveyConfiguration config) {
            if (config != null && config.Data != null) {
                _data = config.Data;
                _results.SurveyId = _data.surveyId;
                Debug.Log($"Survey initialized from ScriptableObject with {_data.pages.Count} pages.");
                ShowPage(0);
            }
            else {
                Debug.LogError("SurveyConfiguration provided to Init is null or contains no data.");
            }
        }


        private void LoadSurvey() {
            if (SurveyConfig != null && SurveyConfig.Data != null) {
                _data = SurveyConfig.Data;
                _results.SurveyId = _data.surveyId;
                Debug.Log($"Survey loaded from ScriptableObject with {_data.pages.Count} pages.");
            }
            else if (SurveyJson != null) {
                string json = SurveyJson.text;
                _data = JsonUtility.FromJson<SurveyData>(json);
                if (_data != null) {
                    _results.SurveyId = _data.surveyId;
                    Debug.Log($"Survey loaded from JSON with {_data.pages.Count} pages.");
                }
                else {
                    Debug.LogError("Failed to parse Survey JSON.");
                }
            }
            else {
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
            Invoke(nameof(ForceLayoutUpdate),0.1f);
        }

        public void ForceLayoutUpdate() {
            // Ensure all UI elements have calculated their sizes
            Canvas.ForceUpdateCanvases();

            // Force layout rebuild on the container
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            if (rectTransform != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        /// <summary>
        /// Clears all generated components and resets the survey state.
        /// </summary>
        [ContextMenu("Cleanup")]
        public void Cleanup() {
            ClearPage();
            _currentPageIndex = -1;
            _results.results.Clear();
            _results.SurveyId = string.Empty;
            _data = null;
        }

        /// <summary>
        /// Clears all child objects of the current GameObject and resets all active survey components.
        /// </summary>
        private void ClearPage() {
            while (this.transform.childCount > 0) {
                Transform child = this.transform.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            _activeComponents.Clear();
            ForceLayoutUpdate();
        }

        private void InstantiateComponent(ComponentData compData) {
            GameObject prefab = GetPrefabForType(compData.type);
            if (prefab != null) {
                GameObject go   = Instantiate(prefab, this.transform);
                var        comp = go.GetComponent<SurveyComponent>();
                if (comp != null) {
                    comp.Init(compData);

                    // Restore previous value if it exists
                    var previousResult = _results.results.Find(r => r.componentId == compData.id);
                    if (previousResult != null) {
                        comp.SetValue(previousResult.value);
                    }

                    _activeComponents.Add(comp);
                }
            }
            else {
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
                }
                else {
                    btnComp.AddButton("Submit", "Submit");
                }

                _activeComponents.Add(btnComp);
            }
        }

        private GameObject GetPrefabForType(string type) {
            if (PrefabReferences == null) {
                PrefabReferences = Resources.Load<SurveyPrefabSettings>("SurveyPrefabs");
            }

            return PrefabReferences != null ? PrefabReferences.GetPrefabForType(type) : null;
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
                    OnSurveyFinished?.Invoke();
                    OnSurveyFinishedResult?.Invoke(JsonUtility.ToJson(_results));
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
                    }
                    else {
                        _results.results.Add(new ComponentResult { componentId = comp.Data.id, value = value });
                    }
                }
            }
        }

        public void SubmitSurvey() {
            CollectCurrentPageResults();
            string jsonResults = "";
            try {
                jsonResults = JsonUtility.ToJson(_results, true);
            }
            catch (System.Exception e) {
                Debug.LogError("Failed to serialize results: " + e.Message);
            }

            Debug.Log("Survey Submitted:\n" + jsonResults);

            // Example: Save to persistent data path
            try {
                string savePath = Path.Combine(Application.persistentDataPath, "results.json");
                File.WriteAllText(savePath, jsonResults);
                Debug.Log("Results saved to: " + savePath);
            }
            catch (System.Exception e) {
                Debug.LogWarning("Failed to save results: " + e.Message);
            }

            // If there are more pages (e.g. a 'thank_you' or 'finish_survey' page), show the next one.
            // By default, the survey finishes after the last page unless a 'finish' action is triggered by a component.
            if (_data != null && _currentPageIndex < _data.pages.Count - 1) {
                ShowPage(_currentPageIndex + 1);
            }
            else {
                OnSurveyFinished?.Invoke();
                OnSurveyFinishedResult?.Invoke(jsonResults);
            }
        }
    }

}
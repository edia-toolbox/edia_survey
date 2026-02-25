using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Edia.Survey {

    /// <summary>
    /// A matrix Likert scale question, with multiple rows (topics) and columns (options).
    /// </summary>
    public class LikertMatrixComponent : SurveyComponent {
        [SerializeField] private TextMeshProUGUI ComponentLabel;
        [SerializeField] private Transform LabelsContainer;
        [SerializeField] private GameObject LabelPrefab;
        [SerializeField] private Transform RowsContainer;
        [SerializeField] private GameObject RowPrefab;

        private readonly List<LikertMatrixRow> _rows = new List<LikertMatrixRow>();
        private GridLayoutGroup _gridLayoutLabels;
        
        public override void Init(ComponentData data) {
            base.Init(data);
            if (ComponentLabel != null) ComponentLabel.text = data.label;

            ClearMatrix();

            if (data.options == null || data.subQuestions == null) return;

            // Setup Header Row
            _gridLayoutLabels = LabelsContainer.GetComponent<GridLayoutGroup>();
            if (LabelsContainer != null && LabelPrefab != null) {

                _gridLayoutLabels.constraintCount = data.options.Count;
                
                foreach (var option in data.options) {
                    var go  = Instantiate(LabelPrefab, LabelsContainer);
                    var txt = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (txt != null) txt.text = option;
                }
            }

            // Setup Rows
            foreach (var topic in data.subQuestions) {
                var go        = Instantiate(RowPrefab, RowsContainer);
                var rowScript = go.GetComponent<LikertMatrixRow>();
                if (rowScript != null) {
                    rowScript.Init(topic, data.options);
                    _rows.Add(rowScript);
                }
            }
        }

        private void ClearMatrix() {
            if (LabelsContainer != null) {
                while (LabelsContainer.childCount > 0) {
                    Transform child = LabelsContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }

            if (RowsContainer != null) {
                while (RowsContainer.childCount > 0) {
                    Transform child = RowsContainer.GetChild(0);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }
            }

            _rows.Clear();
        }

        public override string GetValue() {
            var results = _rows
                .Select(r => new { Topic = r.Topic, Value = r.GetValue() })
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => $"{x.Topic}: {x.Value}")
                .ToList();

            return results.Count > 0 ? string.Join("; ", results) : null;
        }
    }

}
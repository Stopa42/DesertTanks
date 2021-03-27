using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using UnityEditor;
using UnityEngine;

namespace RTSTutorial
{
    public class BuildingsDisplay : MonoBehaviour
    {
        [SerializeField] private BuildingSet _buildables;
        [SerializeField] private List<BuildingButton> _buttons;
        [SerializeField] private float _buttonOffset;

        private void OnValidate()
        {
            Rebuild();
        }

        [ContextMenu("Rebuild")]
        public void Rebuild()
        {
            RemoveExcessiveButtons();
            CheckEnoughButtons();
            SetButtons();
        }

        private void RemoveExcessiveButtons()
        {
            for (var i = _buildables._buildings.Length; i < _buttons.Count; i++)
            {
                Debug.Log("Number of building buttons exceeds number of buildables.");
                _buttons.RemoveAt(i);
            }
        }

        private void CheckEnoughButtons()
        {
            if (_buildables._buildings.Length != _buttons.Count)
                Debug.LogWarning("Number of building buttons is less than number of buildables. " +
                                 "Perhaps missing a button reference?");
        }

        private void SetButtons()
        {
            if (_buttons.Count == 0) return;
            var pos = _buttons[0].GetComponent<RectTransform>().anchoredPosition;
            var offset = new Vector2(_buttonOffset, 0f);
            for (var i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                
                MoveButton(button, pos);
                pos += offset;

                button.SetForBuilding(_buildables._buildings[i], i);
            }
        }

        private void MoveButton(BuildingButton button, Vector2 pos)
        {
            var rect = button.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
        }
    }
}

using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorial
{
    public class UnitSelectionHandler : NetworkBehaviour
    {
        [SerializeField] private LayerMask _unitLayer;
        [SerializeField] private RectTransform _unitSelectionArea;
        [SerializeField] private BuilderPreviewHandler _previewHandler;

        private RTSPlayer _player;
        private Camera _camera;
        private Raycaster _raycaster;
        private Vector2 _startSelectionPoint;

        private readonly List<Unit> _selectedUnits = new List<Unit>();

        public IEnumerable<Unit> SelectedUnits => _selectedUnits.AsReadOnly();

        private Controls _playerInput;
        private bool _isSelecting;
        private bool _selectionModifier;

        private void Start()
        {
            _camera = Camera.main;
            _raycaster = new Raycaster();
            _raycaster.LayerMask = _unitLayer;

            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

            _playerInput = new Controls();
            _playerInput.Player.Enable();

            _playerInput.Player.Select.performed += HandleSelection;
            _playerInput.Player.ModifySelection.performed += ModifySelection;

            GameOverHandler.ClientOnGameOver += HandleGameOver;

            _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            
            GameOverHandler.ClientOnGameOver -= HandleGameOver;
        }

        private void HandleSelection(InputAction.CallbackContext context)
        {
            if (_previewHandler.IsSelected) ClearSelection();
            if (context.ReadValueAsButton()) 
                StartSelection();
            else
                ClearSelection();
        }

        private void ModifySelection(InputAction.CallbackContext context)
        {
            _selectionModifier = context.ReadValueAsButton();
        }

        private void Update()
        {
            if (_isSelecting) UpdateSelectionArea();
        }

        private void StartSelection()
        {
            if (!_selectionModifier) DeselectAll();
            _startSelectionPoint = _playerInput.Player.CursorPosition.ReadValue<Vector2>();
            _unitSelectionArea.gameObject.SetActive(true);
            _isSelecting = true;
            ClickUnitSelect();
        }

        private void DeselectAll()
        {
            foreach (var selectedUnit in _selectedUnits) selectedUnit.Deselect();
            _selectedUnits.Clear();
        }

        private void UpdateSelectionArea()
        {
            var mouseSelectionPoint = _playerInput.Player.CursorPosition.ReadValue<Vector2>();

            var width = Mathf.Abs(_startSelectionPoint.x - mouseSelectionPoint.x);
            var height = Mathf.Abs(_startSelectionPoint.y - mouseSelectionPoint.y);

            _unitSelectionArea.sizeDelta = new Vector2(width, height);
            _unitSelectionArea.anchoredPosition = (_startSelectionPoint + mouseSelectionPoint) / 2;
        }

        private void ClearSelection()
        {
            if (_isSelecting) BoxUnitSelect();
            _isSelecting = false;
            _unitSelectionArea.gameObject.SetActive(false);
        }

        private void BoxUnitSelect()
        {
            var start = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
            var end = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

            foreach (var unit in _player.Units)
            {
                var screenPosition = _camera.WorldToScreenPoint(unit.transform.position);
                if (screenPosition.IsInsideRectangle(start, end)) SelectUnit(unit);
            }
        }

        private void ClickUnitSelect()
        {
            var isSomethingHit = _raycaster.CursorRaycast(out var hit);
            if (!isSomethingHit || !hit.collider.TryGetComponent(out Unit unit)) return;
            if (!unit.hasAuthority) return;
            if (_selectedUnits.Contains(unit))
            {
                DeselectUnit(unit);
            }
            else
            {
                SelectUnit(unit);
            }
        }

        private void SelectUnit(Unit unit)
        {
            _selectedUnits.Add(unit);
            unit.Select();
        }

        private void DeselectUnit(Unit unit)
        {
            if (!_selectedUnits.Contains(unit)) return;
            _selectedUnits.Remove(unit);
            unit.Deselect();
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            _selectedUnits.Remove(unit);
        }

        private void HandleGameOver(int winner)
        {
            enabled = false;
        }
    }

    public static partial class ExtensionMethods
    {
        public static bool IsInsideRectangle(this Vector3 point, Vector2 start, Vector2 end)
        {
            var point2D = new Vector2(point.x, point.y);
            return point2D.IsInsideRectangle(start, end);
        }

        public static bool IsInsideRectangle(this Vector2 point, Vector2 start, Vector2 end)
        {
            var min = Mathf.Min(start.x, end.x);
            var max = Mathf.Max(start.x, end.x);
            var isInXBand = point.x > min && point.x < max;
            min = Mathf.Min(start.y, end.y);
            max = Mathf.Max(start.y, end.y);
            var isInYBand = point.y > min && point.y < max;
            return isInXBand && isInYBand;
        }
    }
}
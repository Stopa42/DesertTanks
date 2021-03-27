using System;
using System.Security.Cryptography;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorial
{
    public class BuilderPreviewHandler : MonoBehaviour
    {
        [SerializeField] private BuildingSet _buildables;
        [SerializeField] private LayerMask _floorMask;
        [SerializeField] private float _rotationSpeed = 0.1f;
        
        private Raycaster _raycaster;
        private Controls _controls;
        private BuilderBehaviour _builderBehaviour;
        private Preview _currentPreview;
        private int _typeId;
        private bool _isSelected;
        private bool _isPlaced;
        private Vector2 _placedCursorPosition;
        private Vector3 _placedPosition;
        private float _rotationAngle;

        public bool IsSelected => _isSelected;
        
        public void SelectBuilding(int typeId)
        {
            _currentPreview.GameObject.SetActive(false);
            
            _typeId = typeId;
            _currentPreview = _buildables.GetPreview(typeId);
            _currentPreview.GameObject.SetActive(true);
            _isSelected = true;
            _isPlaced = false;
            _rotationAngle = 0.0f;
            _currentPreview.RotatePreview(_rotationAngle);
        }

        private void Awake()
        {
            _buildables.BuildPreviews();
        }

        private void Start()
        {
            _raycaster = new Raycaster();
            _raycaster.LayerMask = _floorMask;
            
            _controls = new Controls();
            _controls.Player.Enable();
            _controls.Player.Select.performed += HandleSelection;
            _controls.Player.Command.performed += HandleDeselection;
            
            _builderBehaviour = NetworkClient.connection.identity.GetComponent<BuilderBehaviour>();

            _currentPreview = _buildables.GetPreview(0);
        }

        private void Update()
        {
            if (IsSelected) UpdateBuildingPreview();
        }

        private void UpdateBuildingPreview()
        {
            if (!_isPlaced)
                UpdatePosition();
            else
                UpdateRotation();
        }

        private void UpdatePosition()
        {
            if (!_raycaster.CursorRaycast(out var hit)) return;

            _currentPreview.GameObject.transform.position = hit.point;

            _placedPosition = hit.point;
            SetAllowance();
        }

        private void SetAllowance()
        {
            var canPlaceBuilding = _builderBehaviour.CanPlaceBuilding(_placedPosition, _currentPreview.BoundingBox,
                _currentPreview.GameObject.transform.rotation);
            _currentPreview.SetAllowed(canPlaceBuilding);
        }

        private void UpdateRotation()
        {
            var pos = _controls.Player.CursorPosition.ReadValue<Vector2>();
            _rotationAngle = (pos.x - _placedCursorPosition.x) * _rotationSpeed;
            _currentPreview.RotatePreview(_rotationAngle);

            SetAllowance();
        }

        private void HandleSelection(InputAction.CallbackContext context)
        {
            if (!IsSelected) return;
            if (context.ReadValueAsButton()) PlacePreview();
            else ConfirmPlacement();
        }

        private void HandleDeselection(InputAction.CallbackContext obj)
        {
            if (!IsSelected) return;
            Deselect();
        }

        private void PlacePreview()
        {
            _placedCursorPosition = _controls.Player.CursorPosition.ReadValue<Vector2>();
            if (!_raycaster.CursorRaycast(out var hit))
            {
                Deselect();
                return;
            }
            _placedPosition = hit.point;
            _isPlaced = true;
        }

        private void ConfirmPlacement()
        {
            _builderBehaviour.CmdTryPlaceBuilding(_typeId, _placedPosition, _rotationAngle);
            Deselect();
        }

        private void Deselect()
        {
            _currentPreview.GameObject.SetActive(false);
            _isSelected = false;
        }
    }
}
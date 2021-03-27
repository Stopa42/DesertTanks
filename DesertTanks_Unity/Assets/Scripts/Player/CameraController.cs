using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorial
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _screenBorderThickness = 10f;
        [SerializeField] private float _cameraZOffset = -6f;
        
        private Vector2 _screenXLimits = Vector2.zero;
        private Vector2 _screenZLimits = Vector2.zero;

        private Controls _controls;

        private Vector2 _playerInput;

        [TargetRpc]
        public void TargetFocusCameraAt(Vector3 point) => FocusCameraAt(point); 
        
        public void FocusCameraAt(Vector3 point)
        {
            _cameraTransform.position = new Vector3(point.x, _cameraTransform.position.y, point.z + _cameraZOffset);
        }

        public void SetScreenLimits(Bounds bounds)
        {
            _screenXLimits = new Vector2(bounds.min.x, bounds.max.x);
            _screenZLimits = new Vector2(bounds.min.z + _cameraZOffset, bounds.max.z + _cameraZOffset);
        }

        public override void OnStartAuthority()
        {
            _cameraTransform.gameObject.SetActive(true);

            _controls = new Controls();
            _controls.Enable();

            _controls.Player.MoveCamera.performed += SetPreviousInput;
            _controls.Player.MoveCamera.canceled += SetPreviousInput;
        }

        private void SetPreviousInput(InputAction.CallbackContext context) => _playerInput = context.ReadValue<Vector2>();

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) return;

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            var pos = _cameraTransform.position;
            if (_playerInput == Vector2.zero)
            {
                var cursorMovement = GetCursorMovement();

                pos += cursorMovement.normalized * (_speed * Time.deltaTime);
            }
            else
            {
                pos += new Vector3(_playerInput.x, 0f, _playerInput.y) * (_speed * Time.deltaTime);
            }

            pos.x = Mathf.Clamp(pos.x, _screenXLimits.x, _screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, _screenZLimits.x, _screenZLimits.y);

            _cameraTransform.position = pos;
        }

        private Vector3 GetCursorMovement()
        {
            var cursorMovement = Vector3.zero;

            var cursorPosition = _controls.Player.CursorPosition.ReadValue<Vector2>();

            var inTop = cursorPosition.y >= Screen.height - _screenBorderThickness;
            var inBottom = cursorPosition.y <= _screenBorderThickness;
            
            if (inTop)
            {
                cursorMovement.z += 1;
            }
            else if (inBottom)
            {
                cursorMovement.z -= 1;
            }

            var inRight = cursorPosition.x >= Screen.width - _screenBorderThickness;
            var inLeft = cursorPosition.x <= _screenBorderThickness;
            
            if (inRight)
            {
                cursorMovement.x += 1;
            }
            else if (inLeft)
            {
                cursorMovement.x -= 1;
            }

            return cursorMovement;
        }
    }
}
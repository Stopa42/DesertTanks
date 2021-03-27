using UnityEngine;

namespace RTSTutorial
{
    public class Raycaster
    {
        private Controls _controls;
        private readonly Camera _mainCamera = Camera.main;

        public LayerMask LayerMask { get; set; } = 0;

        public Raycaster()
        {
            _controls =  new Controls();
            _controls.Player.Enable();
        }

        public bool CursorRaycast(out RaycastHit hit)
        {
            var cursorPosition = _controls.Player.CursorPosition.ReadValue<Vector2>();
            var ray = _mainCamera.ScreenPointToRay(cursorPosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask);
        }
    }
}
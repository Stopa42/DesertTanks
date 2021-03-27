using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTSTutorial
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform _minimapRect;
        [SerializeField] private BoxCollider _floor;
        [SerializeField] private Camera _minimapCamera;

        private CameraController _playerCamera;
        
        private float _mapXMin;
        private float _mapXMax;
        private float _mapZMin;
        private float _mapZMax;

        private void Start()
        {
            var bounds = _floor.bounds;
            _mapXMin = bounds.min.x;
            _mapZMin = bounds.min.z;
            _mapXMax = bounds.max.x;
            _mapZMax = bounds.max.z;
            
            _minimapCamera.orthographicSize = Mathf.Max(_mapXMax - _mapXMin, _mapZMax - _mapZMin) / 2;
            
            if (NetworkClient.connection?.identity == null) return;
            _playerCamera = NetworkClient.connection.identity.GetComponent<CameraController>();
            _playerCamera.SetScreenLimits(bounds);
        }

        public void OnPointerDown(PointerEventData eventData) => MoveCamera();

        public void OnDrag(PointerEventData eventData) => MoveCamera();

        private void MoveCamera()
        {
            var mousePosition = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_minimapRect, 
                mousePosition, 
                null,
                out var localPoint)) return;

            var rect = _minimapRect.rect;
            var lerpX = (localPoint.x - rect.x) / rect.width; 
            var lerpZ = (localPoint.y - rect.y) / rect.height;

            var focusAtPoint = new Vector3(
                Mathf.Lerp(_mapXMin, _mapXMax, lerpX), 
                0f,
                Mathf.Lerp(_mapZMin, _mapZMax, lerpZ));
            
            _playerCamera.FocusCameraAt(focusAtPoint);
        }
    }
}
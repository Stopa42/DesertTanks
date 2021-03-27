using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorial
{
    public class UnitCommandGiver : NetworkBehaviour
    {
        [SerializeField] private UnitSelectionHandler _selectionHandler;
        [SerializeField] private LayerMask _targetableLayers;
        [SerializeField] private LayerMask _movableLayers;

        private Raycaster _moveRaycaster;
        private Raycaster _targetRaycaster;

        private Controls _controls;

        private void Start()
        {
            _moveRaycaster = new Raycaster();
            _moveRaycaster.LayerMask = _movableLayers;

            _targetRaycaster = new Raycaster();
            _targetRaycaster.LayerMask = _targetableLayers;
            
            _controls = new Controls();
            _controls.Player.Enable();
            _controls.Player.Command.performed += ExecuteCommand;

            GameOverHandler.ClientOnGameOver += HandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= HandleGameOver;
        }

        private void ExecuteCommand(InputAction.CallbackContext callbackContext)
        {
            if (TryTarget()) return;
            TryMove();
        }

        private void TryMove()
        {
            var isTerrainHit = _moveRaycaster.CursorRaycast(out var hit);
            if (!isTerrainHit) return;
            Move(hit.point);
        }

        private bool TryTarget()
        {
            var isTargetHit = _targetRaycaster.CursorRaycast(out var hit);
            if (!isTargetHit) return false;
            
            var isTargetable = hit.collider.TryGetComponent(out ITargetable target);
            if (!isTargetable) return false;
            
            var isEnemy = !target.IsFriendly;
            if (!isEnemy) return false;
            
            Target(target);
            return true;
        }

        private void Target(ITargetable target)
        {
            foreach (var selectedUnit in _selectionHandler.SelectedUnits)
                selectedUnit.Target(target);
        }

        private void Move(Vector3 destination)
        {
            foreach (var selectedUnit in _selectionHandler.SelectedUnits)
                selectedUnit.Move(destination);
        }

        private void HandleGameOver(int obj)
        {
            enabled = false;
        }
    }
}
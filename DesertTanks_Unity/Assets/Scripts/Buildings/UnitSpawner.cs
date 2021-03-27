using System;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace RTSTutorial
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private Transform _unitSpawnPoint;
        [SerializeField] private int _maxUnitQueue = 5;
        [SerializeField] private float _spawnMoveRange = 7f;
        [SerializeField] private float _unitSpawnDuration = 5f;

        [SyncVar(hook = nameof(OnQueuedUnitsChanged))]
        private int _unitsQueued = 0;

        private float _unitTimer;

        private ResourceHandler _resourceHandler;

        public event Action QueuedUnitsChanged;
        
        public float UnitTimerPercentage => _unitTimer / _unitSpawnDuration;

        public int UnitsQueued => _unitsQueued;

        public override void OnStartServer()
        {
            _resourceHandler = connectionToClient.identity.GetComponent<ResourceHandler>();
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (UnitsQueued == _maxUnitQueue) return;

            if (_resourceHandler.TryPayForObject(_unitPrefab)) _unitsQueued = UnitsQueued + 1;
        }

        private void SpawnUnit()
        {
            var spawnPosition = _unitSpawnPoint.position;
            var unitInstance = Instantiate(_unitPrefab, spawnPosition, _unitSpawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient);

            var spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
            spawnOffset.y = spawnPosition.y;

            var unitMovement = unitInstance.GetComponent<UnitMovement>();

            unitMovement.ServerMove(spawnPosition + spawnOffset);
        }

        private void Update()
        {
            if (isServer) ProduceUnits();
        }

        [Server]
        private void ProduceUnits()
        {
            if (UnitsQueued == 0) return;
            _unitTimer += Time.deltaTime;

            if (_unitTimer < _unitSpawnDuration) return;

            SpawnUnit();
            _unitsQueued = UnitsQueued - 1;
            _unitTimer -= _unitSpawnDuration;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasAuthority) return;
            CmdSpawnUnit();
        }

        private void OnQueuedUnitsChanged(int oldValue, int remainingUnits)
        {
            QueuedUnitsChanged?.Invoke();
        }
    }
}
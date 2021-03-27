using System;
using Mirror;

namespace RTSTutorial
{
    public class UnitBase : NetworkBehaviour
    {
        private Health _health;
        
        public static event Action<UnitBase> ServerOnBaseSpawned;

        public static event Action<int> OnPlayerDefeated;

        public override void OnStartServer()
        {
            ServerOnBaseSpawned?.Invoke(this);
            _health.ServerOnDie += HandleBaseDeath;
        }

        public override void OnStopServer()
        {
            _health.ServerOnDie -= HandleBaseDeath;
        }

        private void HandleBaseDeath()
        {
            var owner = connectionToClient.identity.GetComponent<RTSPlayer>();
            foreach (var building in owner.Buildings)
            {
                if (building.TryGetComponent(out UnitBase unitBase)) return;
            }
            OnPlayerDefeated?.Invoke(connectionToClient.connectionId);
        }

        private void Awake()
        {
            _health = GetComponent<Health>();
        }
    }
}
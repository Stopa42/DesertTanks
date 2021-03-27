using Mirror;

namespace RTSTutorial
{
    public class Suicide : NetworkBehaviour
    {
        private Health _health;
        private void Awake()
        {
            _health = GetComponent<Health>();
        }
        
        public override void OnStartServer()
        {
            UnitBase.OnPlayerDefeated += HandlePlayerDefeat;
        }

        public override void OnStopServer()
        {
            UnitBase.OnPlayerDefeated -= HandlePlayerDefeat;
        }

        private void HandlePlayerDefeat(int owner)
        {
            if (connectionToClient.connectionId != owner) return;
            _health.TakeDamage(_health.CurrentHealth);
        }
    }
}
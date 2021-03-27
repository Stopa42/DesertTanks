using System;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthUpdated;

        public int CurrentHealth => _currentHealth;

        public override void OnStartServer()
        {
            _currentHealth = _maxHealth;
        }

        [Server]
        public void TakeDamage(int damageAmount)
        {
            if (damageAmount <= 0) return;
            if (CurrentHealth == 0) return;

            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

            if (CurrentHealth > 0) return;
            
            ServerOnDie?.Invoke();
        }

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, _maxHealth);
        }
    }
}
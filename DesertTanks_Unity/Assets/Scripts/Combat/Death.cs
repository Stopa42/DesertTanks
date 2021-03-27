using System;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    [RequireComponent(typeof(Health))]
    public class Death : NetworkBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        public override void OnStartServer()
        {
            _health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            _health.ServerOnDie -= ServerHandleDie;
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
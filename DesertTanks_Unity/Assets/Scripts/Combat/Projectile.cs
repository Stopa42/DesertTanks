using System;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class Projectile : NetworkBehaviour
    {
        private Rigidbody _rigidbody;
        [SerializeField] private int _damageToDeal = 20;
        [SerializeField] private float _destroyAfterSeconds = 5f;
        [SerializeField] private float _launchSpeed = 10f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _rigidbody.velocity = _launchSpeed * transform.forward;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), _destroyAfterSeconds);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ITargetable target))
            {
                if (target.IsFriendly) return;
            }
            
            if (other.TryGetComponent(out Health health)) health.TakeDamage(_damageToDeal);
            
            DestroySelf();
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
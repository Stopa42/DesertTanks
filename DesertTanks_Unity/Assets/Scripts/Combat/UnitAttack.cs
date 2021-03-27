using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    [RequireComponent(typeof(Targeter))]
    public class UnitAttack : NetworkBehaviour
    {
        private Targeter _targeter;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnTransform;
        [SerializeField] private float _attackRange = 5f;
        [SerializeField] private float _attackRate = 1f;
        private float _attackDelay;
        [SerializeField] private float _rotationSpeed = 20f;
        private float _lastAttackTime;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _targeter = GetComponent<Targeter>();
            _attackDelay = 1 / _attackRate;
        }

        [ServerCallback]
        private void Update()
        {
            if (!IsTargetInRange()) return;

            RotateToTarget();

            if (CanFire()) Fire();
        }

        private void RotateToTarget()
        {
            var vectorToTarget = _targetPosition - transform.position;
            
            var rotationToTarget = Quaternion.LookRotation(vectorToTarget);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, rotationToTarget, _rotationSpeed * Time.deltaTime);
        }

        private bool CanFire() => Time.time > _lastAttackTime + _attackDelay;

        private void Fire()
        {
            var projectileSpawnPosition = _projectileSpawnTransform.position;
            var rotation = Quaternion.LookRotation(_targetPosition - projectileSpawnPosition);
            var projectile = Instantiate(_projectilePrefab, projectileSpawnPosition, rotation);

            NetworkServer.Spawn(projectile, connectionToClient);

            _lastAttackTime = Time.time;
        }

        [Server]
        private bool IsTargetInRange()
        {
            if (_targeter.Target as Object == null) return false;
            _targetPosition = _targeter.Target.AimAtPoint.position;
            var inRange = (_targetPosition - transform.position).sqrMagnitude <= _attackRange * _attackRange;
            return inRange;
        }
    }
}
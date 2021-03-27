using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTSTutorial
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Targeter _targeter;
        [SerializeField] private float _chaseRange;

        [ServerCallback]
        private void Update()
        {
            CheckForTarget();
            CheckDestination();
        }

        private void CheckForTarget()
        {
            var target = _targeter.Target;
            if (target as Object == null) return;
            var targetPosition = target.gameObject.transform.position;
            if ((targetPosition - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
            {
                _agent.SetDestination(targetPosition);
            }
            else if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }

        private void CheckDestination()
        {
            if (!_agent.hasPath) return;
            if (_agent.remainingDistance > _agent.stoppingDistance) return;
            _agent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position) => ServerMove(position);

        public void ServerMove(Vector3 position)
        {
            var isPositionValid = NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas);
            if (!isPositionValid) return;
            _agent.SetDestination(hit.position);
        }
    }
}

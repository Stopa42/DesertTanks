using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class PointTarget : NetworkBehaviour, ITargetable
    {
        [SerializeField] private Transform _aimAtPoint;
        public Transform AimAtPoint => _aimAtPoint;
        public bool IsFriendly => hasAuthority;
    }
}
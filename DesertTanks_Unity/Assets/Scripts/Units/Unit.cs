using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace RTSTutorial
{
    [RequireComponent(typeof(UnitMovement), typeof(Targeter))]
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnityEvent _onSelect;
        [SerializeField] private UnityEvent _onDeselect;

        private UnitMovement _unitMovement;
        private Targeter _unitTargeter;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;


        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            _onSelect?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            _onDeselect?.Invoke();
        }

        public void Move(Vector3 destination)
        {
            if (isClient)
            {
                _unitTargeter.CmdClearTarget();
                _unitMovement.CmdMove(destination);
            }

            if (isServer)
            {
                _unitTargeter.ServerClearTarget();
                _unitMovement.ServerMove(destination);
            }
        }

        public void Target(ITargetable target) => _unitTargeter.CmdTarget(target.gameObject);

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
        }

        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        private void Awake()
        {
            _unitMovement = GetComponent<UnitMovement>();
            _unitTargeter = GetComponent<Targeter>();
        }

        private void Start()
        {
            _onDeselect?.Invoke();
        }
    }
}

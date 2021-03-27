using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RTSTutorial
{
    public class PartyMember : NetworkBehaviour
    {
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] 
        private string _displayName;
        
        [SyncVar(hook = nameof(AuthorityHandlePartyLeaderUpdated))] 
        private bool _isLeader;

        public static event Action ClientOnInfoUpdated;
        public static event Action<bool> AuthorityOnLeaderStateUpdated;

        public string DisplayName => _displayName;

        public Color TeamColor { get; private set; }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName)
        {
            ClientOnInfoUpdated?.Invoke();
        }
        
        private void AuthorityHandlePartyLeaderUpdated(bool oldState, bool newState)
        {
            if (!hasAuthority) return;
            AuthorityOnLeaderStateUpdated?.Invoke(newState);
            ClientOnInfoUpdated?.Invoke();
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }
        
        [Server]
        public void SetIsLeader(bool state) => _isLeader = state;

        [Command]
        public void CmdStartGame()
        {
            if (!_isLeader) return;
            ((RTSNetworkManager) NetworkManager.singleton).StartGame();
        }

        public override void OnStartServer()
        {
            TeamColor = Random.ColorHSV();
            ClientOnInfoUpdated?.Invoke();
        }

        public override void OnStopServer()
        {
            Destroy(gameObject);
        }

        public override void OnStartClient()
        {
            ClientOnInfoUpdated?.Invoke();
        }

        public override void OnStopClient()
        {
            if (!NetworkServer.active) Destroy(gameObject);
        }

        private void Awake()
        {
            ((RTSNetworkManager) NetworkManager.singleton).Players.Add(this);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            ((RTSNetworkManager) NetworkManager.singleton).Players.Remove(this);
        }
    }
}
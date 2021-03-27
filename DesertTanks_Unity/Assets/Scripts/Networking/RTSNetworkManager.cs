using System;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace RTSTutorial
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject _unitBasePrefab;
        [SerializeField] private GameObject _gameOverHandlerPrefab;
        [SerializeField] private int _minPlayers = 1;

        [Scene] [SerializeField] private string[] _mapScenes;
        [SerializeField] private bool _useSteam;
        [SerializeField] private Transport _steamTransport;
        [SerializeField] private Transport _noSteamTransport;

        private bool _isGameInProgress;
        private List<PartyMember> _players = new List<PartyMember>();

        public List<PartyMember> Players => _players;

        public int MinPlayers => _minPlayers;

        public static event Action OnClientConnectedEvent;
        public static event Action OnClientDisconnectedEvent;

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (!_isGameInProgress) return;
            conn.Disconnect();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<PartyMember>();

            player.SetIsLeader(Players.Count == 1);
            player.SetDisplayName($"Player {Players.Count}");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Players.Clear();
            _isGameInProgress = false;
        }

        [Server]
        public void StartGame()
        {
            if (Players.Count < MinPlayers) return;

            var mapId = Random.Range(0, _mapScenes.Length);
            Debug.Log("Loading Scene " + _mapScenes[mapId]);
            ServerChangeScene(_mapScenes[mapId]);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (_mapScenes.Contains(sceneName)) PrepareMap();
        }

        [Server]
        private void PrepareMap()
        {
            SpawnGameOverHandler();
            foreach (var player in Players)
            {
                var t = GetStartPosition();
                SpawnBase(player, t);
                player.GetComponent<CameraController>().TargetFocusCameraAt(t.position);
            }
            _isGameInProgress = true;
        }

        private void SpawnGameOverHandler()
        {
            var gameOverHandler = Instantiate(_gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandler);
        }

        private void SpawnBase(NetworkBehaviour player, Transform placePosition)
        {
            var unitBase = Instantiate(_unitBasePrefab, 
                placePosition.position, 
                placePosition.rotation * _unitBasePrefab.transform.rotation);
            NetworkServer.Spawn(unitBase, player.connectionToClient);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnectedEvent?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnClientDisconnectedEvent?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }

        public void SetUseSteam(bool enableSteam)
        {
            _useSteam = enableSteam;
            UpdateTransport();
        }

        private void UpdateTransport()
        {
            if (_useSteam)
            {
                _noSteamTransport.Shutdown();
                transport = _steamTransport;
            }
            else
            {
                _steamTransport.Shutdown();
                transport = _noSteamTransport;
            }
            Transport.activeTransport = transport;
            if (NetworkServer.active && NetworkClient.isConnected) StopHost();
            else if (NetworkClient.isConnected) StopClient();
            else if (NetworkServer.active) StopServer();
        }

        public bool GetUseSteam() => _useSteam;

        public override void Start()
        {
            base.Start();
            UpdateTransport();
        }
    }
}

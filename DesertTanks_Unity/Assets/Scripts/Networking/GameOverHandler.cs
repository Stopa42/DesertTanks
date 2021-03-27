using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class GameOverHandler : NetworkBehaviour
    {
        private readonly List<RTSPlayer> _alivePlayers = new List<RTSPlayer>();

        public static event Action ServerOnGameOver;
        public static event Action<int> ClientOnGameOver;
        public override void OnStartServer()
        {
            UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.OnPlayerDefeated += ServerHandlePlayerDefeated;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.OnPlayerDefeated -= ServerHandlePlayerDefeated;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            var player = unitBase.connectionToClient.identity.GetComponent<RTSPlayer>();
            if (_alivePlayers.Contains(player)) return;
            _alivePlayers.Add(player);
        }

        [Server]
        private void ServerHandlePlayerDefeated(int playerID)
        {
            var player = NetworkServer.connections[playerID].identity.GetComponent<RTSPlayer>();
            _alivePlayers.Remove(player);
            if (_alivePlayers.Count > 1) return;
            RpcGameOver(_alivePlayers[0].connectionToClient.connectionId);
            ServerOnGameOver?.Invoke();
        }

        [ClientRpc]
        private void RpcGameOver(int winnerID)
        {
            ClientOnGameOver?.Invoke(winnerID);
        }
    }
}
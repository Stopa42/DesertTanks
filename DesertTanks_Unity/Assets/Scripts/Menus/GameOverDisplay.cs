using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace RTSTutorial.Menus
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winnerNameText;
        [SerializeField] private GameObject _gameOverDisplayParent;

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }
        private void ClientHandleGameOver(int winnerID)
        {
            _gameOverDisplayParent.SetActive(true);
            var winner = $"Player {winnerID} has won!";
            _winnerNameText.text = winner;
        }

        private void Start()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
            _gameOverDisplayParent.SetActive(false);
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }
    }
}
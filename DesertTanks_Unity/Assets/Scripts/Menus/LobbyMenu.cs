using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTSTutorial
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _lobbyUI;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private TMP_Text[] _playerNameTexts = new TMP_Text[4];

        private void Start()
        {
            RTSNetworkManager.OnClientConnectedEvent += HandleClientConnected;
            PartyMember.AuthorityOnLeaderStateUpdated += AuthorityHandleLeaderStateUpdated;
            PartyMember.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.OnClientConnectedEvent -= HandleClientConnected;
            PartyMember.AuthorityOnLeaderStateUpdated -= AuthorityHandleLeaderStateUpdated;
            PartyMember.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void HandleClientConnected()
        {
            _lobbyUI.SetActive(true);
        }

        private void AuthorityHandleLeaderStateUpdated(bool state) => _startGameButton.gameObject.SetActive(state);

        private void ClientHandleInfoUpdated()
        {
            UpdatePlayerNameplates();
        }

        private void UpdatePlayerNameplates()
        {
            var manager = ((RTSNetworkManager) NetworkManager.singleton);
            var players = manager.Players;
            for (var i = 0; i < players.Count; i++)
                _playerNameTexts[i].text = players[i].DisplayName;
            for (var i = players.Count; i < _playerNameTexts.Length; i++)
                _playerNameTexts[i].text = "Waiting For Player...";

            _startGameButton.interactable = players.Count >= manager.MinPlayers;
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<PartyMember>().CmdStartGame();
        }
        
        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
                NetworkManager.singleton.StopHost();
            else if (NetworkServer.active)
                NetworkManager.singleton.StopServer();
            else
                NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
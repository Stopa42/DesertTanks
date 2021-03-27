using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RTSTutorial
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _landingPagePanel;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;

        private void Start()
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                _landingPagePanel.SetActive(true);
                return;
            }

            NetworkManager.singleton.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress",
                SteamUser.GetSteamID().ToString());
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDFriend);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) return;

            var hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress");

            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();

            _landingPagePanel.SetActive(false);
        }

        public void HostLobby()
        {
            _landingPagePanel.SetActive(false);

            if (((RTSNetworkManager)NetworkManager.singleton).GetUseSteam())
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }
            NetworkManager.singleton.StartHost();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
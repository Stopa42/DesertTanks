using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTSTutorial
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _landingPagePanel;
        [SerializeField] private TMP_InputField _addressInput;
        [SerializeField] private Button _joinButton;

        private void OnEnable()
        {
            RTSNetworkManager.OnClientConnectedEvent += HandleClientConnected;
            RTSNetworkManager.OnClientDisconnectedEvent += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            RTSNetworkManager.OnClientConnectedEvent -= HandleClientConnected;
            RTSNetworkManager.OnClientDisconnectedEvent -= HandleClientDisconnected;
        }

        public void Join()
        {
            var address = _addressInput.text;

            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            _joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            _joinButton.interactable = true;
            
            gameObject.SetActive(false);
            _landingPagePanel.SetActive(false);
        }

        public void HandleClientDisconnected()
        {
            _joinButton.interactable = true;
        }
    }
}
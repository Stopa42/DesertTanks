using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace RTSTutorial
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _resourcesText;

        private ResourceHandler _resourceHandler;

        private void Start()
        {
            TryGetPlayer();
        }
        
        private void TryGetPlayer()
        {
            var id = NetworkClient.connection?.identity;
            if (id == null) return;
            if (!id.TryGetComponent<ResourceHandler>(out var handler)) return;
            _resourceHandler = handler;
            ClientHandleResourcesUpdated(_resourceHandler.Resources);
            _resourceHandler.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int resources)
        {
            _resourcesText.text = $"Resources: {resources}";
        }
    }
}
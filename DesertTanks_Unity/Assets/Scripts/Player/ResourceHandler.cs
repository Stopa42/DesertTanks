using System;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class ResourceHandler : NetworkBehaviour
    {
        
        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int _resources = 100;
        
        public event Action<int> ClientOnResourcesUpdated;
        
        public int Resources => _resources;

        [Server]
        public void AddResources(int resourcesToAdd)
        {
            _resources += resourcesToAdd;
        }
        
        [Server]
        public bool TryPayForObject(GameObject thing)
        {
            if (!thing.TryGetComponent(out Purchasable purchasable)) return true;
            var price = purchasable.Price;
            return TryPay(price);
        }

        private bool TryPay(int price)
        {
            if (_resources < price) return false;
            _resources -= price;
            return true;
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }
    }
}
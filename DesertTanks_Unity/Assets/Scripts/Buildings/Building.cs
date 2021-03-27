using System;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    [RequireComponent(typeof(BoxCollider))]
    public class Building : NetworkBehaviour
    {
        [SerializeField] private GameObject _buildingPreview;
        [SerializeField] private Sprite _icon;

        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingUnspawned;

        public static event Action<Building> AuthorityOnBuildingSpawned;
        public static event Action<Building> AuthorityOnBuildingUnspawned;

        public GameObject BuildingPreview => _buildingPreview;
        public Sprite Icon => _icon;
        public BoxCollider PreviewCollider { get; private set; }

        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingUnspawned?.Invoke(this);
        }

        public override void OnStartAuthority()
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!hasAuthority) return;
            AuthorityOnBuildingUnspawned?.Invoke(this);
        }

        private void Awake()
        {
            PreviewCollider = _buildingPreview.GetComponent<BoxCollider>();
        }
    }
}
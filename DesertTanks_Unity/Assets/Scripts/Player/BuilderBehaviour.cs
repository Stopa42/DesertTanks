using System;
using Mirror;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace RTSTutorial
{
    public class BuilderBehaviour : NetworkBehaviour
    {
        [SerializeField] private BuildingSet _buildables;
        [SerializeField] private ResourceHandler _resourceHandler;
        [SerializeField] private LayerMask _buildingBlockLayer;
        [SerializeField] private float _buildingRangeLimit = 5f;

        private RTSPlayer _player;
        private Builder _builder;

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 placePosition, float rotationAngle)
        {
            if (!_buildables.TryGetBuilding(buildingId, out var buildingToPlace)) return;
            Debug.Log("Building found.");

            var preview = _buildables.GetPreview(buildingId);
            preview.RotatePreview(rotationAngle);
            var bounds = preview.BoundingBox;
            var rotation = preview.GameObject.transform.rotation;
            if (!CanPlaceBuilding(placePosition, bounds, rotation)) return;
            Debug.Log("Building can be placed.");

            if (!_resourceHandler.TryPayForObject(buildingToPlace.gameObject)) return;
            Debug.Log("Building paid.");

            PlaceBuilding(placePosition, buildingToPlace, rotation);
            Debug.Log("Building placed.");
        }

        [Server]
        private void PlaceBuilding(Vector3 placePosition, Building buildingToPlace, Quaternion rotation)
        {
            var buildingInstance = Instantiate(buildingToPlace.gameObject, placePosition, rotation);
            NetworkServer.Spawn(buildingInstance, connectionToClient);
        }

        public bool CanPlaceBuilding(Vector3 placePosition, Bounds buildingBlock, Quaternion rotation)
        {
            return _builder.CanPlaceBuilding(placePosition, buildingBlock, rotation);
        }

        private void Awake()
        {
            _player = GetComponent<RTSPlayer>();
            var rangeSquared = _buildingRangeLimit * _buildingRangeLimit;
            _builder = new Builder(_player.Buildings, rangeSquared, _buildingBlockLayer);
        }
    }
}
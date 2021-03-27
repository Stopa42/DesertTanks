using System.Collections.Generic;
using UnityEngine;

namespace RTSTutorial
{
    public class Builder
    {
        private float _buildingRangeLimitSquared;
        private LayerMask _buildingBlockLayer;
        private IEnumerable<Building> _playerBuildings;

        public Builder(IEnumerable<Building> playerBuildings, float buildingRangeLimitSquared, LayerMask buildingBlockLayer)
        {
            _playerBuildings = playerBuildings;
            _buildingRangeLimitSquared = buildingRangeLimitSquared;
            _buildingBlockLayer = buildingBlockLayer;
        }

        public bool CanPlaceBuilding(Vector3 placePosition, Bounds buildingBlock, Quaternion rotation)
        {
            var isInRange = CheckRange(placePosition);

            if (!isInRange) return false;
            
            var isNotBlocked = CheckBuildingBlock(placePosition, buildingBlock, rotation);
            
            return isNotBlocked;
        }

        private bool CheckBuildingBlock(Vector3 placePosition, Bounds buildingBlock, Quaternion rotation)
        {
            var isBlocked = Physics.CheckBox(
                buildingBlock.center + placePosition,
                buildingBlock.extents,
                rotation, 
                _buildingBlockLayer);
            return !isBlocked;
        }

        private bool CheckRange(Vector3 placePosition)
        {
            foreach (var b in _playerBuildings)
            {
                var inRange = (placePosition - b.transform.position).sqrMagnitude <=
                              _buildingRangeLimitSquared;
                if (inRange) return true;
            }
            return false;
        }
    }
}
using System;
using UnityEngine;

namespace RTSTutorial
{
    [CreateAssetMenu(menuName = "Create BuildingList", fileName = "BuildingList", order = 0)]
    public class BuildingSet : ScriptableObject
    {
        public Building[] _buildings;

        private Preview[] _previews;

        public Building GetBuilding(int buildingId)
        {
            if (buildingId < 0 || buildingId > _buildings.Length) return null;
            return _buildings[buildingId];
        }

        public bool TryGetBuilding(int buildingId, out Building building)
        {
            building = GetBuilding(buildingId);
            return building != null;
        }

        public void BuildPreviews()
        {
            _previews = new Preview[_buildings.Length];
            for (var i = 0; i < _buildings.Length; i++)
            {
                var preview = new Preview();
                preview.InitializePreview(_buildings[i].BuildingPreview);
                _previews[i] = preview;
            }
        }

        public Preview GetPreview(int typeId)
        {
            if (typeId < 0 || typeId >= _previews.Length) return null;
            return _previews[typeId];
        }
    }
}
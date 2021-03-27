using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RTSTutorial
{
    public class RTSPlayer : NetworkBehaviour
    {
        public List<Unit> Units { get; } = new List<Unit>();
        public List<Building> Buildings { get; } = new List<Building>();

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingUnspawned += ServerHandleBuildingUnspawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingUnspawned -= ServerHandleBuildingUnspawned;
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (IsOwner(building)) Buildings.Add(building);
        }

        private void ServerHandleBuildingUnspawned(Building building)
        {
            if (IsOwner(building)) Buildings.Remove(building);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (IsOwner(unit)) Units.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (IsOwner(unit)) Units.Remove(unit);
        }

        private bool IsOwner(NetworkBehaviour building)
        {
            return building.connectionToClient.connectionId == connectionToClient.connectionId;
        }

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;
            
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            
            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingUnspawned += AuthorityHandleBuildingUnspawned;
        }

        public override void OnStopClient()
        {
            if (NetworkServer.active) return;
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingUnspawned -= AuthorityHandleBuildingUnspawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit) => Units.Add(unit);
        private void AuthorityHandleUnitDespawned(Unit unit) => Units.Remove(unit);
        
        private void AuthorityHandleBuildingSpawned(Building building) => Buildings.Add(building);
        private void AuthorityHandleBuildingUnspawned(Building building) => Buildings.Remove(building);
    }
}
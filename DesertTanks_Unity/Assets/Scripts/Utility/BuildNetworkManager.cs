using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    /// <summary>
    /// This behaviour prevents trying to instantiate NetworkManager again when main menu scene is loaded
    /// </summary>
    public class BuildNetworkManager : MonoBehaviour
    {
        [SerializeField] private GameObject _networkManagerPrefab;

        private void Awake()
        {
            if (NetworkManager.singleton == null) Instantiate(_networkManagerPrefab);
        }
    }
}

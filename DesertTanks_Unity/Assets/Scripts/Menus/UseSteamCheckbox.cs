using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace RTSTutorial
{
    public class UseSteamCheckbox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _checkBoxX;
        
        private void Start()
        {
            UpdateVisuals();
        }

        public void CheckBoxClick()
        {
            var manager = ((RTSNetworkManager)NetworkManager.singleton);
            var useSteam = manager.GetUseSteam();
            manager.SetUseSteam(!useSteam);
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _checkBoxX.text = ((RTSNetworkManager)NetworkManager.singleton).GetUseSteam() ? "×" : "";
        }
    }
}
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class TeamColor : NetworkBehaviour
    {
        [SerializeField] private bool _autoLoadMaterials = true;
        [SerializeField] private List<Renderer> _renderers = new List<Renderer>();
        [SerializeField] private List<RendererMaterial> _materials = new List<RendererMaterial>();

        [SyncVar(hook = nameof(HandleTeamColorUpdated))]
        private Color _teamColor;

        public override void OnStartClient()
        {
            if (hasAuthority) CmdUpdateColor();
        }

        [Command]
        private void CmdUpdateColor()
        {
            var player = connectionToClient.identity.GetComponent<PartyMember>();
            _teamColor = player.TeamColor;
        }

        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (var r in _materials) r._renderer.materials[r._material].color = newColor;
        }

        private void Start()
        {
            if (_autoLoadMaterials) AutoLoadMaterials();
        }

        private void AutoLoadMaterials()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
                for (var i = 0; i < r.sharedMaterials.Length; i++)
                    _materials.Add(new RendererMaterial(r, i));
        }

        [ContextMenu("Load Materials From Renderers")]
        public void LoadMaterialsFromRenderers()
        {
            _materials.Clear();
            foreach (var r in _renderers)
                for (var i = 0; i < r.sharedMaterials.Length; i++)
                    _materials.Add(new RendererMaterial(r, i));
        }
    }

    [Serializable]
    public struct RendererMaterial
    {
        public Renderer _renderer;
        public int _material;

        public RendererMaterial(Renderer renderer, int material)
        {
            _renderer = renderer;
            _material = material;
        }
    }
}
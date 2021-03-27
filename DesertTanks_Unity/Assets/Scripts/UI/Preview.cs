using System.Collections.Generic;
using UnityEngine;

namespace RTSTutorial
{
    public class Preview
    {
        private readonly List<Material> _materials = new List<Material>();
        private readonly Color _allowedColor = Color.green;
        private readonly Color _notAllowedColor = Color.red;

        private GameObject _previewPrefab;

        public GameObject GameObject { get; private set; }

        public Bounds BoundingBox { get; private set; }

        public void InitializePreview(GameObject previewPrefab)
        {
            
            InstantiatePreview(previewPrefab);
            InitializeMaterials();
            InitializeBoundingBox();
            InitializeTransform();

            GameObject.SetActive(false);
        }

        public void SetAllowed(bool isAllowed)
        {
            var color = isAllowed ? _allowedColor : _notAllowedColor;
            foreach (var m in _materials)
                m.color = color;
        }

        public void RotatePreview(float angle)
        {
            GameObject.transform.rotation = _previewPrefab.transform.rotation * Quaternion.Euler(0.0f, angle, 0.0f);
        }

        private void InstantiatePreview(GameObject previewPrefab)
        {
            _previewPrefab = previewPrefab;
            GameObject = Object.Instantiate(_previewPrefab);
        }

        private void InitializeBoundingBox()
        {
            BoundingBox = GameObject.GetComponent<BoxCollider>().bounds;
        }

        private void InitializeMaterials()
        {
            var renderers = GameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var m in r.materials)
                _materials.Add(m);
        }

        private void InitializeTransform()
        {
            var buildingTransform = _previewPrefab.transform;
            GameObject.transform.localScale = buildingTransform.localScale;
            GameObject.transform.rotation = buildingTransform.rotation;
        }
    }
}
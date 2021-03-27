using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTSTutorial
{
    /// <summary>
    /// Utility component that allows to resize and reposition multiple referenced 'Box' components at the same time.
    /// </summary>
    public class BoxResizer : MonoBehaviour
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private Vector3 _center;
        [SerializeField] private Vector3 _size;

        [SerializeField] private BoxCollider[] _boxColliders;
        [SerializeField] private Transform[] _transforms;
        [SerializeField] private NavMeshObstacle _navMeshObstacle;

        private void OnValidate()
        {
            ApplyTransform();
        }

        private void ApplyTransform()
        {
            if (!_enabled) return;
            
            foreach (var box in _boxColliders)
            {
                box.center = _center;
                box.size = _size;
            }
            
            foreach (var t in _transforms)
            {
                t.localPosition = _center;
                t.localScale = _size;
            }

            if (_navMeshObstacle == null) return;
            _navMeshObstacle.center = _center;
            _navMeshObstacle.size = _size;
        }
    }
}
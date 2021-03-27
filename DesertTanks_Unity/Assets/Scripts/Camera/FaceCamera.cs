using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSTutorial
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform mainCameraTransform;
        
        void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        void LateUpdate()
        {
            var cameraRotation = mainCameraTransform.rotation;
            transform.LookAt(transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
    }
}

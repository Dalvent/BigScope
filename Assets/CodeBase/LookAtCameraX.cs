using UnityEngine;

namespace CodeBase
{
    public class LookAtCameraX : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            Quaternion rotation = _mainCamera.transform.rotation;
            rotation.z = transform.rotation.z;
            rotation.x = transform.rotation.x;
            transform.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.up);
        }
    }
}
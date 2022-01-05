using System;
using UnityEngine;

namespace CodeBase
{
    public class RotateSprite : MonoBehaviour
    {
        public Transform rotationTransform;
        public Sprite FaceSprite;
        public Sprite BackSprite;
        public SpriteRenderer SpriteRenderer;

        private Camera _camera;

        
        public void Awake()
        {
            _camera = Camera.main;
        }

        public void Update()
        {
            var rotation = transform.rotation.y - _camera.transform.rotation.y;
            Debug.Log(rotation);
            if (rotation > 0)
            {
                SpriteRenderer.sprite = FaceSprite;
            }
            else
            {
                SpriteRenderer.sprite = BackSprite;
            }
        }
    }
}
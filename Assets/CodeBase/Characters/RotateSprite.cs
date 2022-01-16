using System;
using System.Collections;
using CodeBase.Extensions;
using UnityEngine;

namespace CodeBase
{
    public class RotateSprite : MonoBehaviour
    {
        public Sprite FaceSprite;
        public Sprite BackSprite;
        public SpriteRenderer SpriteRenderer;
        public Transform ToFlip;

        private Camera _camera;

        private const float LEFT = 270f;
        private const float BACK = 180f;
        private const float RIGHT = 90f;
        private const float FRONT = 0f;

        public void Awake()
        {
            _camera = Camera.main;
        }

        public void Update()
        {
            Debug.Log($"Player - {transform.rotation.eulerAngles.y}, camera {_camera.transform.rotation.eulerAngles.y}");
            var rotation = _camera.transform.eulerAngles.y - transform.rotation.eulerAngles.y;

            // Front left
            if (rotation > LEFT)
            {
                ShowFace();
                RotateTransformLeft();
            }
            // back left
            else if (rotation > BACK)
            {
                ShowBack();
                RotateTransformLeft();
            }
            // Back right
            else if (rotation > RIGHT)
            {
                ShowBack();
                RotateTransformRight();
            }
            // front right
            else
            {
                ShowFace();
                RotateTransformRight();
            }
        }

        private void ShowBack()
        {
            SpriteRenderer.sprite = BackSprite;
        }

        private void ShowFace()
        {
            SpriteRenderer.sprite = FaceSprite;
        }

        private void RotateTransformRight()
        {
            ToFlip.localScale = ToFlip.localScale.SetX(Mathf.Abs(ToFlip.localScale.x));
        }

        private void RotateTransformLeft()
        {
            ToFlip.localScale = ToFlip.localScale.SetX(Mathf.Abs(ToFlip.localScale.x) * -1);
        }
    }
}
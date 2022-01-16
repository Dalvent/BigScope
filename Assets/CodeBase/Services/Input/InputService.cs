using UnityEngine;

namespace CodeBase.Services
{
    public class InputService : IInputService
    {
        private const string Vertical = "Vertical";
        private const string Horizontal = "Horizontal";
        private string Jump = "Jump";
        
        public Vector2 Axis => new Vector2(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));
        public bool IsJumpButtonDown => Input.GetButtonDown(Jump);
    }
}
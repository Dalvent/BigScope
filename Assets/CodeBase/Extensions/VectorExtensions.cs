
using UnityEngine;

namespace CodeBase.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 SetX(this Vector3 vector3, float x)
            => new Vector3(x, vector3.y, vector3.z);
        public static Vector3 SetY(this Vector3 vector3, float y)
            => new Vector3(vector3.x, y, vector3.z);
        public static Vector3 SetZ(this Vector3 vector3, float z)
            => new Vector3(vector3.x, vector3.y, z);
    }
}
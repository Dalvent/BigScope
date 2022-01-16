using UnityEngine;

namespace CodeBase.Extensions
{
    public static class JsonExtensions
    {
        public static T ToDeserializedJson<T>(this string json) => 
            JsonUtility.FromJson<T>(json);
        
        public static string ToJson<T>(this T value) => 
            JsonUtility.ToJson(value);
    }
}
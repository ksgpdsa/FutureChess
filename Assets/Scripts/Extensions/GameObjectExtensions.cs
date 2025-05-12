using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetComponentOnlyInChildren<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponentsInChildren<T>(true).FirstOrDefault(comp => comp.gameObject != obj);
        }
    }
}
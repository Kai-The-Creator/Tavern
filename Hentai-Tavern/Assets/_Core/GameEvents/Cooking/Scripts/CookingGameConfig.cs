// CookingGameConfig.cs   (расширенный)
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    [CreateAssetMenu(fileName="CookingGameConfig", menuName="Configs/Cooking Game")]
    public class CookingGameConfig : ScriptableObject
    {
        [Header("Gameplay")]
        public float CookingDuration = 5f;

        [Header("Order generation")]
        public List<CookRecipeConfig> PossibleRecipes;
        [Min(1)] public int MinOrders = 3;
        [Min(1)] public int MaxOrders = 5;

        [Header("Camera")]
        [Min(.1f)] public float CameraMoveDuration = .8f;
        
        public List<OrderRecord> GenerateOrders()
        {
            var list = new List<OrderRecord>();
            foreach (var t in PossibleRecipes)
            {
                list.Add(new OrderRecord(t, 2)); // 1-2 шт.
            }
            return list;
        }
    }
}
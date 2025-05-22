using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    /// <summary>Скриптабл, у которого есть список rarity-tier’ов.</summary>
    public interface ITieredItemConfig
    {
        /// <returns>Метаданные всех tier’ов; порядок совпадает с Enum <see cref="Rarity"/> (0..4)</returns>
        IReadOnlyList<TierMeta> Metas { get; }

        /// <summary>True, если конфиг описывает tier с такой редкостью.</summary>
        bool HasTier(Rarity r);
    }

    // ──────────────────────────────────────────────────────────────────────────
    [Serializable]
    public struct TierMeta
    {
        public Rarity Rarity; // Common..Legendary
        public Color FrameColor; // цвет рамки/фона
        public Sprite Icon; // мини-иконка
        public bool StartUnlocked;
        public int Price; // на будущее
    }

    /// <summary>Дженерик-контейнер: «Meta + специфичные статы».</summary>
    [Serializable]
    public struct Tier<TPayload> where TPayload : struct
    {
        public TierMeta Meta;
        public TPayload Stats;
    }

    // ─────────── специализированные payload-ы
    [Serializable]
    public struct WeaponTierStats
    {
        [Min(0)] public int BaseDamage;
        [Range(0f, 10f)] public float CritChance;
    }

    [Serializable]
    public struct ArmorTierStats
    {
        [Min(0)] public int Defense;
        [Min(0)] public float Resistance; // %
    }

    [Serializable]
    public struct RingStats
    {
        [Min(0)] public int Heal;
    }
    
    [Serializable]
    public struct AmuletStats
    {
        [Min(0)] public int Heal;
    }
    
    [Serializable]
    public struct PotionStats
    {
        [Min(0)] public int Heal;
    }
}
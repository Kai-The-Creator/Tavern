using UnityEngine;

namespace _Core._Combat
{
    [System.Serializable]
    public class ResourcePool
    {
        public int Health;
        public int Mana;
        public int Stamina;

        public void Clamp(StatBlock stats)
        {
            Health = Mathf.Clamp(Health, 0, stats.MaxHealth);
            Mana = Mathf.Clamp(Mana, 0, stats.MaxMana);
            Stamina = Mathf.Clamp(Stamina, 0, stats.MaxStamina);
        }
    }
}
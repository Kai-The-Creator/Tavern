using UnityEngine;

namespace _Core._Combat
{
    public abstract class PassiveAbilitySO : ScriptableObject
    {
        public virtual void OnTurnStart(CombatEntity entity) { }

        public virtual int ModifyOutgoingPhysical(int damage) => damage;
        public virtual int ModifyOutgoingMagical(int damage) => damage;
        public virtual int ModifyIncomingPhysical(int damage) => damage;
        public virtual int ModifyIncomingMagical(int damage) => damage;
    }
}
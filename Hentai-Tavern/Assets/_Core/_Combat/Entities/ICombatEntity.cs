using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public interface ICombatEntity
    {
        string Id { get; }
        bool IsPlayer { get; }
        StatBlock Stats { get; }
        ResourcePool Resources { get; }
        UniTask OnTurnStart(BattleConfig config);
        UniTask<AbilitySO> SelectAbility();
        Transform Transform { get; }
    }
}

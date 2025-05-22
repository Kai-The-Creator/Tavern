using System.Collections.Generic;

namespace _Core._Combat
{
    public interface IAbilityProvider
    {
        IReadOnlyList<AbilitySO> ActiveAbilities { get; }
        IReadOnlyList<PassiveAbilitySO> PassiveAbilities { get; }
    }
}
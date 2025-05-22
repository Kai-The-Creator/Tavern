// Assets/_Core/_Global/ItemSystem/IStatProvider.cs
using System.Collections.Generic;
using _Core._Global.UI.Tooltips;

namespace _Core._Global.ItemSystem
{
    public interface IStatProvider
    {
        IReadOnlyList<StatPair> GetStats(Rarity tier);
    }
}
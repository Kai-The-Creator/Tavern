namespace _Core._Global.ItemSystem.Behaviours
{
    /// <summary>Базовый контракт для любых стратегий поведения предметов.</summary>
    public interface IItemBehaviour
    {
        /// <remarks>Возвращает <c>true</c>, если данный behaviour обслуживает <paramref name="config"/>.</remarks>
        bool Match(ItemConfig config);

        /// <summary>Попытаться увеличить количество.</summary>
        bool TryAdd(ItemState state, int amount);

        /// <summary>Попытаться израсходовать указанное количество.</summary>
        bool TryConsume(ItemState state, int amount);

        /// <summary>Попытаться разблокировать предмет «как есть» (без учёта tier’ов).</summary>
        bool TryUnlock(ItemState state);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class ClientStats
{
    public Dictionary<StatsType, ChangedValue> stats { get; private set; } = new();

    public void InitDefault()
    {
        stats.Clear();

        ChangedValue HP = new ChangedValue(100, infinite: true);
        ChangedValue STR = new ChangedValue(10, infinite: true);
        ChangedValue INT = new ChangedValue(5, infinite: true);

        stats.Add(StatsType.HP, HP);
        stats.Add(StatsType.STR, STR);
        stats.Add(StatsType.INT, INT);
    }

    public void InitFromSave(
        int hp, int str, int Int
        )
    {
        stats.Clear();

        ChangedValue HP = new ChangedValue(hp, infinite: true);
        ChangedValue STR = new ChangedValue(str, infinite: true);
        ChangedValue INT = new ChangedValue(Int, infinite: true);

        stats.Add(StatsType.HP, HP);
        stats.Add(StatsType.STR, STR);
        stats.Add(StatsType.INT, INT);
    }

    public void Upgrade(StatsType type, int val)
    {
        if (!stats.ContainsKey(type)) return;

        stats[type].Add(val);
    }
}

public enum StatsType
{
    HP,
    STR,
    INT
}

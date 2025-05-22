using UnityEngine;

public static class Client
{
    public static ClientStats Stats { get; private set; }
    public static ChangedValue Money { get; private set; }

    public static void Init()
    {
        Stats = new ClientStats();
        Stats.InitDefault();
        Money = new ChangedValue(100, infinite: true);
    }
}

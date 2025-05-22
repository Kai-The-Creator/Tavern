using UnityEngine;

public class GameEventConfig : CSVLoadConfig
{
    [Space]
    [Header("Event settings")]
    public Quest targetQuest;
    public int state;
}

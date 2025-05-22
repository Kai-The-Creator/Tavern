using System;
using UnityEngine;

[Serializable]
public class IntRange
{
    public int minValue;
    public int maxValue;

    public IntRange(int minValue, int maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public int GetRandom()
    {
        return (int)UnityEngine.Random.Range(minValue, maxValue);
    }
}
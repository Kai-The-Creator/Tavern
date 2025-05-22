using System;
using UnityEngine;

[Serializable]
public class FloatRange
{
    public float minValue;
    public float maxValue;

    public FloatRange(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public float GetRandom()
    {
        return UnityEngine.Random.Range(minValue, maxValue);
    }
}
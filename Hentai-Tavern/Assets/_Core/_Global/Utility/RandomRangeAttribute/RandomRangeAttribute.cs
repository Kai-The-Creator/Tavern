using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class RandomRangeAttribute : PropertyAttribute
{
    public float minLimit;
    public float maxLimit;

    public RandomRangeAttribute(float minLimit, float maxLimit)
    {
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}

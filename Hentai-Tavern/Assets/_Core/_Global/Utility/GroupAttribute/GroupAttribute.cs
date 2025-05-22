using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class GroupAttribute : PropertyAttribute
{
    public string groupName;

    public GroupAttribute(string groupName)
    {
        this.groupName = groupName;
    }
}

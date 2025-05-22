using System;

namespace _Core._Global.Services
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DependsOnAttribute : Attribute
    {
        public Type[] Dependencies { get; }

        public DependsOnAttribute(params Type[] dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
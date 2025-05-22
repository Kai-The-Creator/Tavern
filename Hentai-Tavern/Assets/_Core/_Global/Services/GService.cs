using System;
using System.Collections.Generic;

namespace _Core._Global.Services
{
    public static class GService
    {
        private static readonly Dictionary<Type, object> _byType = new();

        public static void Init()
        {
            _byType.Clear();
        }

        public static void AddService(AService service)
        {
            var svcType = service.GetType();
            _byType[svcType] = service;

            foreach (var iface in svcType.GetInterfaces())
            {
                if (typeof(IService).IsAssignableFrom(iface))
                    _byType[iface] = service;
            }
        }

        public static T GetService<T>() where T : class
        {
            _byType.TryGetValue(typeof(T), out var svc);
            return svc as T;
        }

        public static object GetService(Type type)
        {
            _byType.TryGetValue(type, out var svc);
            return svc;
        }

        public static void ClearServices()
        {
            _byType.Clear();
        }
    }
}
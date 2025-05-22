using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.Services
{
    public class ServicesInitializer : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }

        private void Start()
        {
            InitializeAll().Forget();
        }

        private async UniTask InitializeAll()
        {
            var services = FindObjectsOfType<AService>();

            GService.ClearServices();
            foreach (var svc in services)
                GService.AddService(svc);

            var graph = new Dictionary<AService, List<AService>>();
            foreach (var svc in services)
            {
                var depsList = new List<AService>();
                var attr = svc.GetType().GetCustomAttribute<DependsOnAttribute>();
                if (attr != null)
                {
                    foreach (var depType in attr.Dependencies)
                    {
                        var depSvc = GService.GetService(depType) as AService;
                        if (depSvc != null)
                            depsList.Add(depSvc);
                    }
                }

                graph[svc] = depsList;
            }

            var inDegree = new Dictionary<AService, int>();
            var reverseAdj = new Dictionary<AService, List<AService>>();
            foreach (var node in graph.Keys)
            {
                inDegree[node] = graph[node].Count;
                reverseAdj[node] = new List<AService>();
            }

            foreach (var node in graph.Keys)
            foreach (var dep in graph[node])
                reverseAdj[dep].Add(node);

            var queue = new Queue<AService>();
            var result = new List<AService>();
            foreach (var kv in inDegree)
                if (kv.Value == 0)
                    queue.Enqueue(kv.Key);

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                result.Add(n);

                foreach (var m in reverseAdj[n])
                {
                    inDegree[m]--;
                    if (inDegree[m] == 0)
                        queue.Enqueue(m);
                }
            }

            if (result.Count != graph.Count)
                Debug.LogError("ServicesInitializer: cycle detected in service dependencies!");

            foreach (var svc in result)
            {
                Debug.Log($"ServicesInitializer: initializing {svc.GetType().Name}");
                await svc.OnStart();
            }

            IsInitialized = true;
            Debug.Log("ServicesInitializer: all services initialized");
        }
    }
}
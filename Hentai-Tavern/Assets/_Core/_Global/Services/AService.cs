using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.Services
{
    public abstract class AService : MonoBehaviour
    {
        private void Awake()
        {
            GService.AddService(this);
        }

        public abstract UniTask OnStart();
    }
}
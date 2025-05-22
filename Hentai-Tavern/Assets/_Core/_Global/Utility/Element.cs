using UnityEngine;

namespace _Core._Global.Utility
{
    public class Element : MonoBehaviour
    {
        public virtual void Activate()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
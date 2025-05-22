using UnityEngine;

namespace _Core._Global.CameraService
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0, 4, -6);
        [SerializeField] private float smooth = 0.15f;
        private Vector3 vel;

        public void SetTarget(Transform t) => target = t;
        public void ClearTarget() => target = null;

        private void LateUpdate()
        {
            if (!target) return;
            var desired = target.position + target.rotation * offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, smooth);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 0.2f);
        }
    }
}
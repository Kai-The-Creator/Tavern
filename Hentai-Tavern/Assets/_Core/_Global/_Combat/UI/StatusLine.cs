using UnityEngine;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Follows a combat entity and displays its status effects.
    /// </summary>
    public class StatusLine : MonoBehaviour
    {
        [SerializeField] private StatusIndicator indicator;
        [SerializeField] private Vector3 worldOffset = Vector3.up * 2.5f;

        private CombatEntity _target;
        private Camera _camera;

        public StatusIndicator Indicator => indicator;

        /// <summary>
        /// Assigns the entity this status line should follow.
        /// </summary>
        public void Bind(CombatEntity target)
        {
            _target = target;
            _camera = Camera.main;
            if (indicator == null)
                indicator = GetComponentInChildren<StatusIndicator>();

            if (_target != null)
            {
                var ctrl = _target.GetComponent<StatusController>();
                if (ctrl != null)
                    ctrl.SetIndicator(indicator);
                indicator?.SetPassives(_target.Passives);
            }
        }

        private void LateUpdate()
        {
            if (_target == null || _camera == null) return;
            transform.position = _camera.WorldToScreenPoint(_target.transform.position + worldOffset);
        }
    }
}
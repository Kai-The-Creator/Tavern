using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Displays a health bar for a combat entity on a screen space canvas.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider bar;
        [SerializeField] private Vector3 worldOffset = Vector3.up * 2f;

        private CombatEntity _target;
        private Camera _camera;

        /// <summary>
        /// Assigns the entity this bar should follow.
        /// </summary>
        public void SetTarget(CombatEntity target)
        {
            _target = target;
            _camera = Camera.main;
            UpdateBar();
        }

        private void LateUpdate()
        {
            if (_target == null || _camera == null || bar == null) return;

            UpdateBar();
            var pos = _camera.WorldToScreenPoint(_target.transform.position + worldOffset);
            transform.position = pos;
        }

        private void UpdateBar()
        {
            if (bar != null && _target != null)
                bar.value = (float)_target.Resources.Health / _target.Stats.MaxHealth;
        }
    }
}
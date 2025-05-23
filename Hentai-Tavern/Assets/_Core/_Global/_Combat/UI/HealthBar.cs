using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Displays a health bar for a combat entity on a screen space canvas.
    /// </summary>
    [RequireComponent(typeof(CombatEntity))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider barPrefab;
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Vector3 worldOffset = Vector3.up * 2f;

        private CombatEntity _entity;
        private Slider _bar;
        private Camera _camera;

        private void Awake()
        {
            _entity = GetComponent<CombatEntity>();
            _camera = Camera.main;
            if (barPrefab != null && targetCanvas != null)
            {
                _bar = Instantiate(barPrefab, targetCanvas.transform);
                UpdateBar();
            }
        }

        private void LateUpdate()
        {
            if (_entity == null || _bar == null || _camera == null) return;

            var ratio = (float)_entity.Resources.Health / _entity.Stats.MaxHealth;
            _bar.value = ratio;
            var pos = _camera.WorldToScreenPoint(_entity.transform.position + worldOffset);
            _bar.transform.position = pos;
        }

        private void UpdateBar()
        {
            if (_bar != null)
                _bar.value = (float)_entity.Resources.Health / _entity.Stats.MaxHealth;
        }
    }
}

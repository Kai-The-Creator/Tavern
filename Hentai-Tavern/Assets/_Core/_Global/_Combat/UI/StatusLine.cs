using UnityEngine;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Spawns a StatusIndicator prefab on a screen-space canvas and tracks the
    /// owning combat entity.
    /// </summary>
    [RequireComponent(typeof(CombatEntity))]
    public class StatusLine : MonoBehaviour
    {
        [SerializeField] private StatusIndicator indicatorPrefab;
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Vector3 worldOffset = Vector3.up * 2.5f;

        private CombatEntity _entity;
        private StatusIndicator _indicator;
        private Camera _camera;

        private void Awake()
        {
            _entity = GetComponent<CombatEntity>();
            _camera = Camera.main;
            if (indicatorPrefab != null && targetCanvas != null)
            {
                _indicator = Instantiate(indicatorPrefab, targetCanvas.transform);
                _indicator.SetPassives(_entity.Passives);
                var controller = _entity.GetComponent<StatusController>();
                if (controller != null)
                    controller.SetIndicator(_indicator);
            }
        }

        private void LateUpdate()
        {
            if (_indicator == null || _entity == null || _camera == null) return;
            var pos = _camera.WorldToScreenPoint(_entity.transform.position + worldOffset);
            _indicator.transform.position = pos;
        }
    }
}

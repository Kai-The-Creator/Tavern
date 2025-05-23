using UnityEngine;

namespace _Core._Combat
{
    /// <summary>
    /// Simple component that raises an event when the entity is clicked.
    /// Requires a collider.
    /// </summary>
    public class ClickableTarget : MonoBehaviour
    {
        public event System.Action<CombatEntity> OnClicked;

        private CombatEntity _entity;

        private void Awake()
        {
            _entity = GetComponent<CombatEntity>();
        }

        private void OnMouseDown()
        {
            if (_entity != null)
                OnClicked?.Invoke(_entity);
        }
    }
}

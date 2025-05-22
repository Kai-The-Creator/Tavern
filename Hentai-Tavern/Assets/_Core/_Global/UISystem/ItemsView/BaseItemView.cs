using _Core._Global.ItemSystem;
using _Core._Global.UISystem.Filtered;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Core._Global.UISystem.ItemsView
{
    public abstract class BaseItemView<TContext> : MonoBehaviour, IPoolable, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        [Header("Appear Animation")] [SerializeField]
        private float appearDuration = 0.3f;

        [SerializeField] private Ease appearEase = Ease.OutBack;

        [Header("Hover Animation")] [SerializeField]
        private float hoverScale = 1.05f;

        [SerializeField] private float hoverDuration = 0.2f;
        protected ItemConfig CurrentConfig { get; set; }
        private bool _hasAppeared;

        public virtual void OnSpawn(in TContext ctx) { }

        public virtual void OnDespawn() { }

        public abstract void UpdateView();

        public void AnimateIn()
        {
            if (_hasAppeared) return;
            _hasAppeared = true;
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, appearDuration).SetEase(appearEase);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(hoverScale, hoverDuration);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(1f, hoverDuration);
        }

        public virtual void OnPointerClick(PointerEventData eventData) { }
    }
}
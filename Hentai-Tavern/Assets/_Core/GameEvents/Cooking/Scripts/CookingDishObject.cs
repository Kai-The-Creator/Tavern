using DG.Tweening;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class CookingDishObject : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Анимирует уменьшение масштаба блюда до нуля за `duration` секунд и возвращает Tween,
        /// чтобы можно было подписаться на OnComplete.
        /// </summary>
        public Tween Serve(float duration)
        {
            // Здесь вы можете выбрать любую Ease-кривую
            return transform.DOScale(0f, duration).SetEase(Ease.InBack);
        }
    }
}
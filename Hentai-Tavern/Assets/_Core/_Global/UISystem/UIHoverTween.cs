// Assets/_Core/_Global/UI/Common/UIHoverTween.cs
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Core._Global.UISystem
{
    /// <summary>Базовый ховер-эффект: scale / цвет / alpha по событиям Pointer.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIHoverTween :
        MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler,  IPointerUpHandler
    {
        [Header("Targets")]
        [Tooltip("Если null – масштабируется сам RectTransform.")]
        [SerializeField] private RectTransform _scaleTarget;
        [SerializeField] private Graphic[]     _colorTargets;   // Image / TMP-Text …

        [Header("Scale")]
        [SerializeField] private Vector3 _hoverScale  = Vector3.one * 1.1f;
        [SerializeField] private Vector3 _pressScale  = Vector3.one * 0.95f;
        [SerializeField] private float   _duration    = .15f;

        [Header("Color")]
        [SerializeField] private Color   _hoverTint   = Color.white;
        [SerializeField] private Color   _pressTint   = new(.8f, .8f, .8f);
        [SerializeField] private bool    _animateColor = true;

        private Sequence _seq;
        private Vector3 _initialScale;
        private Color[] _initialColors;

        private void Awake()
        {
            if (_scaleTarget == null) _scaleTarget = (RectTransform)transform;
            _initialScale = _scaleTarget.localScale;

            if (_colorTargets == null || _colorTargets.Length == 0)
                _animateColor = false;

            if (_animateColor)
            {
                _initialColors = new Color[_colorTargets.Length];
                for (int i = 0; i < _colorTargets.Length; i++)
                    _initialColors[i] = _colorTargets[i].color;
            }
        }

        private void OnDestroy() => KillSequence();

        // ────────────────────── Pointer events
        public void OnPointerEnter(PointerEventData _) => PlayTween(_hoverScale, _hoverTint);
        public void OnPointerExit (PointerEventData _) => PlayTween(_initialScale, GetInitialTint());
        public void OnPointerDown (PointerEventData _) => PlayTween(_pressScale, _pressTint);
        public void OnPointerUp   (PointerEventData _) => PlayTween(_hoverScale, _hoverTint);

        // ────────────────────── core
        private void PlayTween(Vector3 toScale, Color toColor)
        {
            KillSequence();

            _seq = DOTween.Sequence()
                   .Join(_scaleTarget.DOScale(toScale, _duration).SetUpdate(true));

            if (_animateColor)
            {
                for (int i = 0; i < _colorTargets.Length; i++)
                {
                    var g = _colorTargets[i];
                    _seq.Join(g.DOColor(toColor, _duration).SetUpdate(true));
                }
            }
        }

        private void KillSequence()
        {
            _seq?.Kill();
            _seq = null;
        }

        private Color GetInitialTint() => _animateColor ? _initialColors[0] : Color.white;
    }
}

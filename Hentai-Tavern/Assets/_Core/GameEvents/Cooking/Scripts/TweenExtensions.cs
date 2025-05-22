// TweenExtensions.cs
// Общие утилиты для работы с DOTween + CancellationToken
using System.Threading;
using DG.Tweening;
using UnityEngine;

namespace _Core.Common.Utils
{
    public static class TweenExtensions
    {
        /// <summary>Привязывает твин к токену: при Cancel → Kill(false).</summary>
        public static T LinkToToken<T>(this T t, CancellationToken token) where T : Tween
        {
            if (!t.active) return t;
            token.Register(() => { if (t.active) t.Kill(false); });
            return t;
        }

        /// <summary>Лёгкий shake-эффект для UI-кнопок.</summary>
        public static void ShakeButton(this Transform tr, float dur = .35f, float strength = 20f) =>
            tr.DOShakePosition(dur, strength, 25, 90, false, true);
    }
}
// CookingIngredientObject.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CookingIngredientObject : MonoBehaviour
{
    [SerializeField] private GameObject wholeModel;
    [SerializeField] private GameObject choppedModel;

    private Camera _mainCamera;
    
    private void Awake()
    {
        wholeModel.SetActive(true);
        choppedModel.SetActive(false);
    }

    /// <summary>
    /// Полный цикл «нарезки» – кликаем по коллайдеру нужное число раз,
    /// после чего меняем модель и улетаем к trashPoint.
    /// </summary>
    public async UniTask Chop(int requiredClicks,
                              Transform trashPoint, Camera mainCamera,
                              CancellationToken extToken = default)
    {
        _mainCamera = mainCamera;
        int   done   = 0;
        float yScale = transform.localScale.y;

        // linked Token – обрываем, если объект уничтожен или вызывается StopGame
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(
                            this.GetCancellationTokenOnDestroy(),
                            extToken);

        while (done < requiredClicks)
        {
            // ждём клика ЛКМ
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0),
                                    cancellationToken: cts.Token);

            // рейкаст в точку клика
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) continue;
            if (hit.transform != transform)         continue;

            await AnimateHit(yScale);
            done++;
        }

        // ► заменяем на нарезанную часть
        wholeModel.SetActive(false);
        choppedModel.SetActive(true);

        await transform.DOPunchScale(Vector3.one * 0.3f, 0.25f);
        
        // ► улетаем к мусорке и растворяемся
        await transform.DOMove(trashPoint.position, .35f).SetEase(Ease.InQuad);
        await transform.DOScale(0f, .25f);
    }

    /// небольшое «прижатие» + лёгкий punch-scale
    private async UniTask AnimateHit(float originalY)
    {
        DOTween.Kill(transform); // сброс предыдущих твинов
        var seq = DOTween.Sequence().Join(transform.DOPunchScale(Vector3.one * 0.1f, 0.1f));
        await seq.Play();
    }
}

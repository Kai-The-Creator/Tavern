using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    /// <summary>
    /// Placeholder controller for selecting multiple targets.
    /// Currently selects the first N enemies and briefly highlights them.
    /// </summary>
    public class TargetSelectionController : MonoBehaviour
    {
        [SerializeField] private float highlightTime = 0.5f;

        public async UniTask<IReadOnlyList<ICombatEntity>> SelectTargets(IEnumerable<CombatEntity> candidates, int max)
        {
            var list = new List<ICombatEntity>();

            var valid = candidates.Where(c => c != null).ToList();
            if (max <= 1)
            {
                var tcs = new UniTaskCompletionSource<CombatEntity>();
                void Clicked(CombatEntity c) => tcs.TrySetResult(c);

                foreach (var c in valid)
                {
                    c.GetComponent<TargetIndicator>()?.SetSelected(true);
                    var clickable = c.GetComponent<ClickableTarget>();
                    if (clickable != null)
                        clickable.OnClicked += Clicked;
                }

                var result = await tcs.Task;

                foreach (var c in valid)
                {
                    c.GetComponent<TargetIndicator>()?.SetSelected(false);
                    var clickable = c.GetComponent<ClickableTarget>();
                    if (clickable != null)
                        clickable.OnClicked -= Clicked;
                }

                if (result != null)
                    list.Add(result);
            }
            else
            {
                var sel = valid.Take(max).ToList();
                foreach (var c in sel)
                    c.GetComponent<TargetIndicator>()?.SetSelected(true);
                if (highlightTime > 0f)
                    await UniTask.Delay((int)(highlightTime * 1000));
                foreach (var c in sel)
                    c.GetComponent<TargetIndicator>()?.SetSelected(false);
                list.AddRange(sel);
            }

            return list;
        }
    }
}

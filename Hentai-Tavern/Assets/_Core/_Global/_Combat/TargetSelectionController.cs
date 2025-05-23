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
            var list = candidates.Where(c => c != null).Take(max).ToList();
            foreach (var c in list)
            {
                c.GetComponent<TargetIndicator>()?.SetSelected(true);
            }
            if (highlightTime > 0f)
                await UniTask.Delay((int)(highlightTime * 1000));
            foreach (var c in list)
            {
                c.GetComponent<TargetIndicator>()?.SetSelected(false);
            }
            return list.Cast<ICombatEntity>().ToList();
        }
    }
}

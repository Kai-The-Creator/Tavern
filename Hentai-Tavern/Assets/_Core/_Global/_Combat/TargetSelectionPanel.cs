using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat
{
    public class TargetSelectionPanel : MonoBehaviour
    {
        [SerializeField] private Button targetButtonPrefab;
        [SerializeField] private Transform container;

        private readonly List<Button> _spawned = new();
        private UniTaskCompletionSource<CombatEntity> _tcs;

        public async UniTask<CombatEntity> ChooseTarget(IReadOnlyList<CombatEntity> targets)
        {
            Clear();
            _tcs = new UniTaskCompletionSource<CombatEntity>();
            foreach (var t in targets)
            {
                if (t.Resources.Health <= 0) continue;
                var btn = Instantiate(targetButtonPrefab, container);
                btn.GetComponentInChildren<Text>().text = t.name;
                btn.onClick.AddListener(() => Select(t));
                _spawned.Add(btn);
            }
            return await _tcs.Task;
        }

        private void Select(CombatEntity target)
        {
            _tcs.TrySetResult(target);
            Clear();
        }

        private void Clear()
        {
            foreach (var b in _spawned)
                Destroy(b.gameObject);
            _spawned.Clear();
        }
    }
}


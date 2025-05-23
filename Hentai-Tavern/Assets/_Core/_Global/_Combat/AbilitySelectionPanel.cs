using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Combat.UI;

namespace _Core._Combat
{
    public class AbilitySelectionPanel : MonoBehaviour
    {
        [SerializeField] private AbilityButton abilityButtonPrefab;
        [SerializeField] private Transform container;

        private readonly List<AbilityButton> _spawned = new();
        private UniTaskCompletionSource<AbilitySO> _tcs;

        public async UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities, Func<AbilitySO, int> cooldownProvider = null)
        {
            Clear();
            _tcs = new UniTaskCompletionSource<AbilitySO>();
            foreach (var ability in abilities)
            {
                var btn = Instantiate(abilityButtonPrefab, container);
                var cd = cooldownProvider?.Invoke(ability) ?? 0;
                btn.Setup(ability, cd);
                btn.Button.onClick.AddListener(() => Select(ability));
                _spawned.Add(btn);
            }
            return await _tcs.Task;
        }

        private void Select(AbilitySO ability)
        {
            _tcs.TrySetResult(ability);
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

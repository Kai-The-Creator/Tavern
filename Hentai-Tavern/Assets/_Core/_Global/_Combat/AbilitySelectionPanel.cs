using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat
{
    public class AbilitySelectionPanel : MonoBehaviour
    {
        [SerializeField] private Button abilityButtonPrefab;
        [SerializeField] private Transform container;

        private readonly List<Button> _spawned = new();
        private UniTaskCompletionSource<AbilitySO> _tcs;

        public async UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities, Func<AbilitySO, int> cooldownProvider = null)
        {
            Clear();
            _tcs = new UniTaskCompletionSource<AbilitySO>();
            foreach (var ability in abilities)
            {
                var btn = Instantiate(abilityButtonPrefab, container);
                var cd = cooldownProvider?.Invoke(ability) ?? 0;
                btn.GetComponentInChildren<TextMeshProUGUI>().text = cd > 0 ? $"{ability.name} ({cd})" : ability.name;
                btn.interactable = cd <= 0;
                btn.onClick.AddListener(() => Select(ability));
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

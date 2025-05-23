using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public async UniTask<AbilitySO> ChooseAbility(IReadOnlyList<AbilitySO> abilities)
        {
            Clear();
            _tcs = new UniTaskCompletionSource<AbilitySO>();
            foreach (var ability in abilities)
            {
                var btn = Instantiate(abilityButtonPrefab, container);
                btn.GetComponentInChildren<Text>().text = ability.name;
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

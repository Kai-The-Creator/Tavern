using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Core._Global.Categories
{
    public class SortDropdownView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private readonly List<Action> callbacks = new List<Action>();

        public void Build(List<(string Label, Action Callback)> options)
        {
            Clear();
            var labels = new List<string>();
            foreach (var opt in options)
            {
                labels.Add(opt.Label);
                callbacks.Add(opt.Callback);
            }

            dropdown.AddOptions(labels);

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(index =>
            {
                if (index >= 0 && index < callbacks.Count)
                    callbacks[index]?.Invoke();
            });
        }

        public void Clear()
        {
            dropdown.ClearOptions();
            callbacks.Clear();
        }
    }
}
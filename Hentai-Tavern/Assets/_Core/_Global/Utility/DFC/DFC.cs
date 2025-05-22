using System;
using System.Collections.Generic;

namespace DataFunctionCollection
{
    public class DFC
    {
        public static DFC get;

        private Dictionary<CTData, Dictionary<string, Action>> dictionaries = new();

        public DFC()
        {
            get = this;
        }

        public void Add(CTData key)
        {
            if (!dictionaries.ContainsKey(key))
            {
                dictionaries[key] = new Dictionary<string, Action>();
            }
        }

        public void Remove(CTData key)
        {
            if (dictionaries.ContainsKey(key))
            {
                dictionaries.Remove(key);
            }
        }

        private Dictionary<string, Action> GetDictionary(CTData key)
        {
            if (dictionaries.ContainsKey(key))
            {
                return dictionaries[key];
            }
            throw new KeyNotFoundException($"Dictionary with key '{key}' does not exist.");
        }

        public Dictionary<string, Action> this[CTData key]
        {
            get => GetDictionary(key);
        }

        public void Execute(CTData keyCollection, string subKey)
        {
            if (dictionaries.ContainsKey(keyCollection))
            {
                var dictionary = dictionaries[keyCollection];
                if (dictionary != null && dictionary.ContainsKey(subKey))
                {
                    dictionary[subKey]?.Invoke();
                }
            }
        }
    }
}
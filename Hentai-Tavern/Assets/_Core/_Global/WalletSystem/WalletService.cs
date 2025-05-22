using System;
using System.Collections.Generic;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.WalletSystem
{
    public class WalletService : AService, IWalletService
    {
        [SerializeField] private List<CurrencyBalance> startBalances;

        private readonly Dictionary<string, int> _balance = new();

        public event Action<string, int> OnBalanceChanged;

        public override async UniTask OnStart()
        {
            _balance.Clear();
            foreach (var cb in startBalances) _balance[cb.currencyId] = Mathf.Max(0, cb.amount);

            await UniTask.CompletedTask;
        }

        public bool CanSpend(Money price) =>
            _balance.TryGetValue(price.CurrencyId, out var bal) && bal >= price.Amount;

        public bool Spend(Money price)
        {
            if (!CanSpend(price)) return false;
            _balance[price.CurrencyId] -= price.Amount;
            OnBalanceChanged?.Invoke(price.CurrencyId, _balance[price.CurrencyId]);
            return true;
        }

        public void Earn(Money reward)
        {
            _balance.TryAdd(reward.CurrencyId, 0);
            _balance[reward.CurrencyId] += reward.Amount;
            OnBalanceChanged?.Invoke(reward.CurrencyId, _balance[reward.CurrencyId]);
        }

        public int GetBalance(string currencyId) =>
            _balance.GetValueOrDefault(currencyId, 0);
    }

    [System.Serializable]
    public struct CurrencyBalance
    {
        public string currencyId;
        public int amount;
    }
}
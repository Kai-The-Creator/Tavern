using System;
using UnityEngine;

namespace _Core._Global.WalletSystem
{
    [Serializable]
    public struct Money : IEquatable<Money>
    {
        public string CurrencyId;
        public int Amount;

        public Money(string id, int amount)
        {
            CurrencyId = id;
            Amount = Mathf.Max(0, amount);
        }

        public bool Equals(Money other) => CurrencyId == other.CurrencyId && Amount == other.Amount;

        public override string ToString() => $"{Amount} {CurrencyId}";
    }
}
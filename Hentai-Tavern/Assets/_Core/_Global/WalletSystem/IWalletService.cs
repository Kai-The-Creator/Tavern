using System;
using _Core._Global.Services;

namespace _Core._Global.WalletSystem
{
    public interface IWalletService : IService
    {
        event Action<string, int> OnBalanceChanged;

        bool CanSpend(Money price);
        bool Spend(Money price);
        void Earn(Money reward);
        int GetBalance(string currencyId);
    }
}
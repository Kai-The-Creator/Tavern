using Cysharp.Threading.Tasks;

namespace _Core.GameEvents.Enchantment.States
{
    public interface IEnchantmentState
    {
        UniTask Enter();
        void    Update();
        UniTask Exit();
    }
}
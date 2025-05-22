using _Core._Global.Categories;
using _Core._Global.Services;
using _Core._Global.UISystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.GConfig
{
    public class GlobalConfigService : AService, IGlobalConfigService
    {
        [field: SerializeField] public LanguageConfig LanguageConfig { get; private set; }
        [field: SerializeField] public LocalizationUIConfig LocalizationUIConfig { get; private set; }
        [field: SerializeField] public CharactersList Characters { get; private set; }
        [field: SerializeField] public LocationsList Locations { get; private set; }
        [field: SerializeField] public UIList Windows { get; private set; }
        [field: SerializeField] public CategoryList Categories { get; private set; }

        public override UniTask OnStart()
        {
            return UniTask.CompletedTask;
        }
    }
}
using _Core._Global.Categories;
using _Core._Global.Services;
using _Core._Global.UISystem;

namespace _Core._Global.GConfig
{
    public interface IGlobalConfigService : IService
    {
        public LanguageConfig LanguageConfig { get; }
        public LocalizationUIConfig LocalizationUIConfig { get; }
        public CharactersList Characters { get; }
        public LocationsList Locations { get; }
        public UIList Windows { get; }
        public CategoryList Categories { get; }
    }
}

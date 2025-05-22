using UnityEngine;

namespace _Core._Global.Categories.Filtering
{
    [CreateAssetMenu(fileName = "NewCategory", menuName = "GAME/Categories/CategoryDefinition")]
    public class CategoryDefinition : ScriptableObject
    {
        [SerializeField] private string categoryId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        public string CategoryId => categoryId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}
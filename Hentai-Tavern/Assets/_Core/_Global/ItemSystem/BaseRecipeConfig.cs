// BaseRecipeConfig.cs

using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    /// <summary>Common fields for any recipe item.</summary>
    public abstract class BaseRecipeConfig : ItemConfig, IWorkstationRecipe
    {
        [Header("Workstations")]
        [Tooltip("Craft stations where this recipe can be used.")]
        [SerializeField] private WorkstationTag[] _workstations;

        public IReadOnlyList<WorkstationTag> Workstations => _workstations;

        public override bool Stackable => false;
    }
}
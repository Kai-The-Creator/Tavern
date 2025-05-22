// _Core/GameEvents/Common/ProductModel.cs
using UnityEngine;

namespace _Core.GameEvents.Common
{
    /// <summary>Generic 3-D model for mini-games.
    /// If blend-shapeCount>0 — supports progressive reveal,
    /// otherwise instantly shows final mesh.</summary>
    public sealed class ProductModel : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer mesh;
        [SerializeField] private int blendShapeIndex;

        public bool  HasBlend => mesh != null && mesh.sharedMesh.blendShapeCount > 0;

        public void ResetBlend() { if (HasBlend) SetBlend(0); }
        public void SetBlend(float v01to100)
        {
            if (HasBlend) mesh.SetBlendShapeWeight(blendShapeIndex, Mathf.Clamp(v01to100, 0, 100));
        }

        public Renderer Renderer => mesh;
    }
}
using UnityEngine;

namespace _Core.GameEvents.Forging.Data
{
    [CreateAssetMenu(fileName = "ForgeGameConfig", menuName = "GAME/Configs/ForgeGame")]
    public class ForgeGameConfig : ScriptableObject
    {
        [Header("Lives")]
        [Min(1)]  [SerializeField] private int  totalLives      = 3;
        [SerializeField]          private bool infiniteLives   = false;

        [Header("Target spawning")]
        [Min(0f)] [SerializeField] private float minSpawnDelay  = .5f;
        [Min(0f)] [SerializeField] private float maxSpawnDelay  = 2f;
        [Min(1)]  [SerializeField] private int   maxTargets     = 5;
        [Min(.1f)][SerializeField] private float targetLifetime = 3f;

        [Header("Spawn area relative to model bounds")]
        [Min(0f)] [SerializeField] private float offsetLeft   = .5f;
        [Min(0f)] [SerializeField] private float offsetRight  = .5f;
        [Min(0f)] [SerializeField] private float offsetTop    = .5f;
        [Min(0f)] [SerializeField] private float offsetBottom = .5f;

        [Header("Points & Blend-shape")]
        [Min(0)]  [SerializeField] private int   pointsFirstHalf  = 5;
        [Min(0)]  [SerializeField] private int   pointsSecondHalf = 3;
        [Range(0,100)][SerializeField] private float maxBlend     = 100f;
        [Min(.1f)][SerializeField] private float blendPerPoint    = 1f;

        [Header("VFX / SFX")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private GameObject hitParticles;

        [Header("Overlap check")]
        [Min(0)] [SerializeField] private float targetRadius   = 40f;
        [Min(1)]  [SerializeField] private int   maxSpawnTry    = 10;

        #region Public getters
        public int   TotalLives      => totalLives;
        public bool  InfiniteLives   => infiniteLives;
        public float MinSpawnDelay   => minSpawnDelay;
        public float MaxSpawnDelay   => maxSpawnDelay;
        public int   MaxTargets      => maxTargets;
        public float TargetLifetime  => targetLifetime;
        public float OffsetLeft      => offsetLeft;
        public float OffsetRight     => offsetRight;
        public float OffsetTop       => offsetTop;
        public float OffsetBottom    => offsetBottom;
        public int   PointsFirstHalf => pointsFirstHalf;
        public int   PointsSecondHalf=> pointsSecondHalf;
        public float MaxBlend        => maxBlend;
        public float BlendPerPoint   => blendPerPoint;
        public AudioClip  HitSound   => hitSound;
        public GameObject HitFx      => hitParticles;
        public float TargetRadius    => targetRadius;
        public int   MaxSpawnTry     => maxSpawnTry;
        #endregion
    }
}

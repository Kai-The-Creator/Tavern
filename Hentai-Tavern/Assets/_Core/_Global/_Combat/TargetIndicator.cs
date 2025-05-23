using UnityEngine;

namespace _Core._Combat
{
    /// <summary>
    /// Simple component to toggle selection indicator on combat entities.
    /// Used as a placeholder for target selection visuals.
    /// </summary>
    public class TargetIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject indicator;

        private void Awake()
        {
            if (indicator != null) indicator.SetActive(false);
        }

        public void SetSelected(bool value)
        {
            if (indicator != null) indicator.SetActive(value);
        }
    }
}

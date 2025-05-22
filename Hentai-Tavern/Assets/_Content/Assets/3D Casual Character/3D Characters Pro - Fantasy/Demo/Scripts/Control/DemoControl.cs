using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class DemoControl : MonoBehaviour
    {
        public static DemoControl Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        [field: SerializeField] public PresetData PresetData { get; set; } // ScriptableObject 참조

        private void Start()
        {
            Character.Instance.Init();
            UIControl.Instance.Init();
            Character.Instance.OnRandomChanged.Invoke();
        }

    }
}
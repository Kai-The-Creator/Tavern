using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public enum PartType
    {
        None,
        Head,      // 머리
        Hair,       //머리카락
        Eyebrows,    // 눈썹
        Eyes,      // 눈
        Mouth,     // 입
        Beard,     // 수염
        Glasses,    // 안경
        Mask,      // 마스크
        Earrings,    // 귀걸이
        Back,      // 등 (가방)
        Chest,   // 상의
        Legs,   // 하의
        Feet,     // 신발
        Hands,     //장갑
        Wrists, // 손장식
        LeftWeapon,    // 왼손
        RightWeapon    // 오른손
    }
    
    public class Character : MonoBehaviour
    {
        public static Character Instance { get; private set; }
        [SerializeField] private CharacterPart[] parts;

        public Action<PartType, int> OnPartChanged;
        public Action<PartType, bool> OnPartActiveChanged;
        public Action OnRandomChanged;
        public Action OnNextPart;
        public Action OnPreviousPart;
        public Action OnPreset;
        // public Action<PartType, string> OnEquipItem;

        [SerializeField] private Animator animator;

        private void Awake()
        {
            Instance = this;
        }

        public void Init()
        {
            parts = transform.GetComponentsInChildren<CharacterPart>();
            foreach (var part in parts) part.SetPart();
        }

        public List<GameObject> CurrentPartsType(PartType type)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].PartType == type)
                {
                    return parts[i].CurrentPartsObjects;
                }
            }
            
            return new List<GameObject>();
        }


        public Dictionary<PartType, int> CurrentPartsTypeAndNameList()
        {
            var saveData = new Dictionary<PartType, int>();
            for (var i = 0; i < parts.Length; i++)
            {
                saveData.Add(parts[i].PartType, parts[i].CurrentIndex); 
            }

            return saveData;
        }
        
        public CharacterPart CurrentCharacterPartByType(PartType type) => parts.FirstOrDefault(p => p.PartType == type);
        
        private void OnDisable()
        {
            OnPartChanged = null;
            OnPartActiveChanged = null;
            OnRandomChanged = null;
        }
    
        public void PlayAnimation(AnimationClip clip)
        {
            animator.CrossFadeInFixedTime(clip.name, 0.25f);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class CharacterPart : MonoBehaviour
    {
        [field: SerializeField] public PartType PartType { get; set; } = PartType.None;
        public List<GameObject> CurrentPartsObjects { get; set; }= new();
        [field: SerializeField] public bool IsOnlyEquip { get; set; }
        public int CurrentIndex { get; set; } = -1;

        public string CurrentName() => CurrentIndex == -1 ? "" : CurrentPartsObjects[CurrentIndex].name;
        private int CurrentMaxIndex => CurrentPartsObjects.Count - 1;
        
        public void SetPart()
        {
            foreach (Transform child in transform)
            {
                CurrentPartsObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }

            Character.Instance.OnPartActiveChanged += OnChangeActive;
            Character.Instance.OnPartChanged += OnPartChanged;
            Character.Instance.OnRandomChanged += OnRandomChanged;
            Character.Instance.OnNextPart += OnNextPart;
            Character.Instance.OnPreviousPart += OnPreviousPart;
        }

        private void OnNextPart()
        {
            if(UIControl.Instance.FocusPartTYpe != PartType) return;
            if (CurrentIndex < CurrentMaxIndex)
            {
                CurrentIndex++;
            }
            else
            {
                CurrentIndex = 0;   
            }

            
            SetPartByIndex();
            Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
        }

        public void SetPartByIndex()
        {
            HideAllParts();

            try
            {
                if(CurrentIndex < 0) return;
                CurrentPartsObjects[CurrentIndex].SetActive(true);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }



        /// <summary>
        /// 모든 파츠 끄기
        /// </summary>
        private void HideAllParts()
        {
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }
        
        
        
        #region Event
        
        private void OnPreviousPart()
        {
            if(UIControl.Instance.FocusPartTYpe != PartType) return;
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
            else
            {
                CurrentIndex = CurrentMaxIndex;   
            }
            
            SetPartByIndex();
            Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
        }

        
        private void OnChangeActive(PartType type, bool active)
        {
            if(type != PartType) return;
            gameObject.SetActive(!active);
        }

        private void OnPartChanged(PartType type, int index)
        {
            if(type != PartType) return;
            CurrentIndex = index;
            SetPartByIndex();
        }

        private void OnRandomChanged()
        {
            HideAllParts();

            if (!IsOnlyEquip && Random.value < 0.5f)
            {
                Character.Instance.OnPartChanged(PartType, -1);
                CurrentIndex = -1;
                return;
            }

            CurrentIndex = Random.Range(0, CurrentPartsObjects.Count);
            SetPartByIndex();
            Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
        }
        #endregion
        
 

    }
}

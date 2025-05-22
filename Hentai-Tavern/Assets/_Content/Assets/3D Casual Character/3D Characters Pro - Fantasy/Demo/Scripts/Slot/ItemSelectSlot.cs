using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class ItemSelectSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image imageItem; 
        private PartType _partType;
        private int _index;

        public void SetSlot(int index, string partsName, PartType partType)
        {
            _index = index;
            _partType = partType;

            imageItem.sprite = UIControl.GetSprite($"ScreenShot/{partsName}");
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Character.Instance.OnPartChanged.Invoke(_partType, _index);
        }
    }
}
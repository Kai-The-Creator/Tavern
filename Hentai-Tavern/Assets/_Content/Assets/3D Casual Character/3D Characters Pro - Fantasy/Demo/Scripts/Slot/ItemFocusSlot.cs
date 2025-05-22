using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class ItemFocusSlot : MonoBehaviour
    {
        public void OnClick_Next()
        {
            Character.Instance.OnNextPart.Invoke();
        }

        public void OnClick_Previous()
        {
            Character.Instance.OnPreviousPart.Invoke();
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
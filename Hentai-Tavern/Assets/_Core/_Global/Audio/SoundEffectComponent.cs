using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEffectComponent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioSystem.instance.SetSound(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioSystem.instance.SetSound(hoverSound);
    }
}

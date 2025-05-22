using UnityEngine;


public class GameEvent : MonoBehaviour
{
    public Events eventType;

    protected bool isPlaying;

    public virtual void Play()
    {
        if (isPlaying)
        {
            Debug.LogWarning($"[GameEvent] Событие '{eventType}' уже запущено. Повторный Play()?");
            return;
        }

        isPlaying = true;
        Debug.Log($"[GameEvent] '{eventType}' -> Play()");

        Activate();
    }

    public virtual void CompleteEvent()
    {
        if (!isPlaying)
        {
            Debug.LogWarning($"[GameEvent] '{eventType}' CompleteEvent() вызван, хотя isPlaying = false.");
        }

        isPlaying = false;
        Deactivate();
    }

    public virtual void Stop()
    {
        if (isPlaying)
        {
            isPlaying = false;
            Deactivate();
            Debug.Log($"[GameEvent] '{eventType}' принудительно остановлено (Stop).");
        }
    }

    protected virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    protected virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
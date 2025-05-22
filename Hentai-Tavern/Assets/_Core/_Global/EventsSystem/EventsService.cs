using System.Collections.Generic;
using _Core._Global.EventsSystem;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventsService : AService, IEventsService
{
    [Header("Список префабов ивентов (каждый должен иметь уникальный eventType)")]
    [SerializeField] private List<GameEvent> eventPrefabs;

    private Dictionary<Events, GameEvent> eventInstanceMap = new Dictionary<Events, GameEvent>();

    private GameEvent currentEvent;
    private bool isEventPlaying;

    public bool IsEventPlaying => isEventPlaying;
    public GameEvent CurrentEvent => currentEvent;

    public override async UniTask OnStart()
    {
        eventInstanceMap.Clear();

        if (eventPrefabs == null || eventPrefabs.Count == 0)
        {
            Debug.LogWarning("[EventsManager] Нет префабов ивентов.");
            return;
        }

        foreach (var prefab in eventPrefabs)
        {
            if (!prefab)
            {
                Debug.LogWarning("[EventsManager] Пустая ссылка на префаб ивента!");
                continue;
            }

            if (eventInstanceMap.ContainsKey(prefab.eventType))
            {
                Debug.LogWarning($"[EventsManager] Дублируется GameEvent.eventType = {prefab.eventType}! Пропускаем.");
                continue;
            }

            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);

            eventInstanceMap[prefab.eventType] = instance;

            Debug.Log($"[EventsManager] Сконструирован ивент '{prefab.eventType}'");
        }

        await UniTask.Yield();
        Debug.Log("[EventsManager] Все ивенты загружены/созданы.");
    }

    public void PlayEvent(Events e)
    {
        if (isEventPlaying)
        {
            Debug.LogWarning($"[EventsManager] Уже идёт событие '{currentEvent?.eventType}', не можем запустить '{e}'.");
            return;
        }

        if (!eventInstanceMap.TryGetValue(e, out var gameEvent))
        {
            Debug.LogWarning($"[EventsManager] Ивент '{e}' не найден среди инстансированных!");
            return;
        }

        currentEvent = gameEvent;
        isEventPlaying = true;
        currentEvent.Play();

        Debug.Log($"[EventsManager] Событие '{currentEvent.eventType}' запущено.");
    }

    public void CompleteCurrentEvent()
    {
        if (!isEventPlaying)
        {
            Debug.LogWarning("[EventsManager] Нет активного события для завершения.");
            return;
        }

        if (currentEvent)
        {
            currentEvent.CompleteEvent();
            Debug.Log($"[EventsManager] Событие '{currentEvent.eventType}' завершено (из Manager).");
        }

        currentEvent = null;
        isEventPlaying = false;
    }

    public void ForceStopCurrentEvent()
    {
        if (!isEventPlaying) return;

        if (currentEvent)
        {
            currentEvent.Stop();
            Debug.Log($"[EventsManager] Событие '{currentEvent.eventType}' форс-остановлено.");
        }

        currentEvent = null;
        isEventPlaying = false;
    }
}

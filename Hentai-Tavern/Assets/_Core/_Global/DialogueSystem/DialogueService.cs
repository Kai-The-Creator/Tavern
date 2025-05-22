using System;
using _Core._Global.DialogueSystem;
using _Core._Global.GConfig;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Global.UISystem; 

[DependsOn(typeof(IUIService), typeof(IGlobalConfigService))]
public class DialogueService : AService, IDialogueService
{
    private IUIService _uiService;
    private GlobalConfigService globalConfigService;            
    private DialogueWindow _dialogueWindow;
    private DialogueNode _currentNode;
    private int _currentID;
    private bool _isDialogueActive = false;

    private bool _canClickNext = true; 
    private bool _dialogHasAnswers = false;

    public override async UniTask OnStart()
    {
        await UniTask.Yield();
        
        _uiService = GService.GetService<IUIService>();
        globalConfigService = GService.GetService<GlobalConfigService>();
        
        if (_uiService == null) Debug.LogError("DialogueController: UIService not found!");
        if (globalConfigService == null) Debug.LogError("DialogueController: GConfig not found!");

        Debug.Log("DialogueController: Initialized!");
    }

    public void StartDialogue(DialogueNode node)
    {
        if (node == null)
        {
            Debug.LogError("DialogueController.StartDialogue: node == null!");
            return;
        }

        if (_isDialogueActive)
        {
            if (_currentNode == node)
            {
                // Игнорируем повторный вызов
                Debug.LogWarning("DialogueController: Этот же диалог уже активен. Повторный вызов StartDialogue игнорируем.");
                return;
            }

            EndDialogue();
        }

        _currentNode = node;
        _currentID = 0;
        _isDialogueActive = true;

        _uiService.ShowWindow(WindowType.Dialogue);

        _dialogueWindow = GetDialogueWindow();
        if (_dialogueWindow == null)
        {
            Debug.LogError("DialogueController: Не найдён DialogueWindow в UIService!");
            return;
        }

        _dialogueWindow.Init(this);
        _dialogueWindow.SetAnswers(null);
        SetBackgroundAndCharacter(_currentNode);
        UpdateDialogueUI();
    }

    public async void Next()
    {
        if (!_isDialogueActive || _currentNode == null)
        {
            Debug.LogWarning("DialogueController.Next: диалог не активен или нет _currentNode");
            return;
        }

        if (_dialogHasAnswers)
        {
            Debug.Log("DialogueController.Next: Невозможно перейти по Next, т.к. есть ответы. Нужно выбрать ответ.");
            return;
        }

        if (!_canClickNext) return;
        _canClickNext = false;

        try
        {
            var dNode = _currentNode.GetNode(_currentID);
            if (dNode == null)
            {
                Debug.LogWarning($"DialogueController.Next: Нет DNode для _currentID={_currentID}.");
                EndDialogue();
                return;
            }

            switch (dNode.NextType)
            {
                case NodeNextType.End:
                    EndDialogue();
                    return;
            }

            if (dNode.NextID == -1) 
                _currentID++;
            else 
                _currentID = dNode.NextID;

            if (_currentID >= _currentNode.GetLength)
                EndDialogue();
            else 
                UpdateDialogueUI();
        }
        finally
        {
            // Небольшая задержка
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            _canClickNext = true;
        }
    }

    private void UpdateDialogueUI()
    {
        if (_dialogueWindow == null)
        {
            Debug.LogWarning("DialogueController.UpdateDialogueUI: окно диалога отсутствует!");
            return;
        }
        if (_currentNode == null)
        {
            Debug.LogWarning("DialogueController.UpdateDialogueUI: _currentNode == null");
            return;
        }

        var dNode = _currentNode.GetNode(_currentID);
        if (dNode == null)
        {
            Debug.LogWarning($"DialogueController.UpdateDialogueUI: Нет DNode для _currentID={_currentID}.");
            EndDialogue();
            return;
        }

        _dialogueWindow.SetDialogueText(dNode.GetText());

        // Можно на каждом шаге менять фон/персонажа:
        // SetBackgroundAndCharacter(dNode);

        if (dNode.answers is { Count: > 0 })
        {
            _dialogueWindow.SetAnswers(dNode.answers);
            _dialogHasAnswers = true;
        }
        else
        {
            _dialogueWindow.SetAnswers(null);
            _dialogHasAnswers = false;
        }
    }

    public async void SelectAnswer(DAnswer answer)
    {
        if (!_isDialogueActive || _currentNode == null || answer == null)
        {
            Debug.LogWarning("DialogueController.SelectAnswer: диалог не активен или answer == null");
            return;
        }

        // Если Answer хранит конкретный NextID
        // (если -1, то вызываем Next())
        if (answer.NextID < 0)
        {
            Next();
            return;
        }

        _dialogHasAnswers = false;
        _canClickNext = false;

        try
        {
            _currentID = answer.NextID;
            if (_currentID >= _currentNode.GetLength)
            {
                EndDialogue();
                return;
            }

            UpdateDialogueUI();
        }
        finally
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            _canClickNext = true;
        }
    }

    private void EndDialogue()
    {
        if (!_isDialogueActive) return;

        var lastNode = (_currentNode != null && _currentID < _currentNode.GetLength) ? _currentNode.GetNode(_currentID) : null;

        if (lastNode != null && lastNode.Event != Events.None)
        {
            var eventsManager = GService.GetService<EventsService>();
            if (eventsManager != null)
            {
                eventsManager.PlayEvent(lastNode.Event);
            }
        }

        _uiService.CloseWindow(WindowType.Dialogue);

        _dialogueWindow = null;
        _currentNode = null;
        _currentID = 0;
        _isDialogueActive = false;
        _dialogHasAnswers = false;
        _canClickNext = true;
    }

    private DialogueWindow GetDialogueWindow()
    {
        if (_uiService is UIService realService)
        {
            var uiWin = realService.GetWindow(WindowType.Dialogue);
            return uiWin as DialogueWindow;
        }
        return null;
    }

    private void SetBackgroundAndCharacter(DialogueNode node)
    {
        if (_dialogueWindow == null || globalConfigService == null || node == null) return;

        if (node.location == Locations.None)
            _dialogueWindow.SetBackground(null);
        else
        {
            var locData = globalConfigService.Locations.GetLocation(node.location);
            _dialogueWindow.SetBackground(locData ? locData.dialogueBG : null);
        }

        if (node.character == Character.None)
            _dialogueWindow.SetCharacter(null);
        else
        {
            var charData = globalConfigService.Characters.GetCharacter(node.character);
            _dialogueWindow.SetCharacter(charData);
        }
    }
}

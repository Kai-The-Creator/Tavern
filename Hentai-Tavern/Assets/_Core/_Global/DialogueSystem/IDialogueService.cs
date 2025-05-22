using _Core._Global.Services;

namespace _Core._Global.DialogueSystem
{
    public interface IDialogueService: IService
    {
        void Next();
        void SelectAnswer(DAnswer answer);
    }
}
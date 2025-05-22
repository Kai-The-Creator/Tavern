using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenBuildAction", menuName = "GAME/Map/OpenBuild")]
public class OpenBuildAction : ScriptableObject
{
    public OBActionType type;
    public DialogueNode standartDialogue;

    public virtual void SendAction()
    {
        DialogueService d = GService.GetService<DialogueService>();
        if (d != null && standartDialogue != null)
        {
            d.StartDialogue(standartDialogue);
        }
    }
}

public enum OBActionType
{
    None,
    Dialogue,
    OpenTavern
}
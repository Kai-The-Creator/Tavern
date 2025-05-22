using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenChurch", menuName = "GAME/Map/OpenChurch")]
public class OpenChurch : OpenBuildAction
{
    public override void SendAction()
    {
        DialogueService d = GService.GetService<DialogueService>();
        if (d != null && standartDialogue != null)
        {
            d.StartDialogue(standartDialogue);
        }
    }
}
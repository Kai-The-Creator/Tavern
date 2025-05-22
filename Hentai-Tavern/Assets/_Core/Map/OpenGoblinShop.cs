using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenGoblinShop", menuName = "GAME/Map/OpenGoblinShop")]
public class OpenGoblinShop : OpenBuildAction
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
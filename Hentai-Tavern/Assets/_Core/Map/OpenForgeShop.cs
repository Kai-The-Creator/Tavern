using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenForgeShop", menuName = "GAME/Map/OpenForgeShop")]
public class OpenForgeShop : OpenBuildAction
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
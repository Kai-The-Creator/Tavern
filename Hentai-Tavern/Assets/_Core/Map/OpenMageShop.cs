using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenMageShop", menuName = "GAME/Map/OpenMageShop")]
public class OpenMageShop : OpenBuildAction
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
using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenPredictorShop", menuName = "GAME/Map/OpenPredictorShop")]
public class OpenPredictorShop : OpenBuildAction
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
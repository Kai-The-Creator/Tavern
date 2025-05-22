using _Core._Global.Services;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenLordCastle", menuName = "GAME/Map/OpenLordCastle")]
public class OpenLordCastle : OpenBuildAction
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
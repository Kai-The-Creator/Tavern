using _Core._Global.GConfig;
using _Core._Global.Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapBuild : Widget, IPointerDownHandler
{
    [SerializeField] private Locations tag;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI logoText;
    [SerializeField] private OpenBuildAction openAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (openAction == null) return;
        openAction.SendAction();
    }

    public void OnStart()
    {
        // К примеру, иконки берём из GConfig
        GlobalConfigService globalConfigService = GService.GetService<GlobalConfigService>();
        var config = globalConfigService.Locations.GetLocation(tag);
        if (config && icon)
        {
            icon.sprite = config.bannerIcon;
        }
        //if (logoText) logoText.text = config.GetName();
    }

    public void OnUpdate()
    {
        // Можно что-то делать каждый кадр, 
        // например, анимации, проверку условий.
    }

    public override void OnUpdateWidget()
    {
        throw new System.NotImplementedException();
    }
}
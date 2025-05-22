using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationConfig", menuName = "GAME/Locations/LocationConfig")]
public class LocationConfig : CSVLoadConfig
{
    public Locations tag;
    public Sprite dialogueBG;
    public Sprite bannerIcon;

    public List<StringLang> locName = new();

    public string GetName()
    {
        return locName.Find(x => x.language == Language.RU).Text;
    }
}

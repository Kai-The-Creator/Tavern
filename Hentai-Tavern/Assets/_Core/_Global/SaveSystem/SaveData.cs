using System;
using System.Collections.Generic;

public class SaveData
{
    //public Locations currentLocation = Locations.Apart_GG_Living_Room;
    public DayTimeType dayTimeType = DayTimeType.Night;
    public WheekDay wheekDay = WheekDay.Sun;
    public string date;

    public CharactersDatingData datingData;
    public List<MessangerTag> messangerCompleteList = new();

    public SaveData()
    {
        date = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
        datingData = new CharactersDatingData();
    }
}

/*[System.Serializable]
public class GallerySaveData
{
    public List<GCard> CardList = new();
    public List<GScene> SceneList = new();
    public List<GArt> ArtList = new();
}*/


[System.Serializable]
public class SaveArrayData
{
    public List<string> data = new List<string>();
}

[System.Serializable]
public class CharactersDatingData
{
    public List<CharDate> datingList = new();
}

[System.Serializable]
public class CharDate
{
    public Character character;
    public int level;
}

[System.Serializable]
public class SettingsData
{
    public int resolutionID;
    public Language language;
    public float soundsVolume;
    public float musicVolume;
    public bool fullScr = true;
}
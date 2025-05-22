using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CutSceneNode", menuName = "GAME/Dialogues/CutSceneNode")]
public class CutSceneNode : CSVLoadConfig
{
    public List<CNode> nodes = new();
    public List<Sprite> images = new();

    public void Clear()
    {
        nodes.Clear();
    }

    public override void SetData(List<Dictionary<string, string>> dataList)
    {
        nodes.Clear();

        for (int i = 0; i < dataList.Count; i++)
        {
            int ID = i;
            CreateNode(dataList, ID);
        }

        SetDirty(this);
    }

    private void CreateNode(List<Dictionary<string, string>> dataList, int ID)
    {
        CNode node = new CNode();

        node.Texts = SetLang(dataList[ID]);

        dataList[ID].TryGetValue("Adult", out string adult);

        if (!string.IsNullOrEmpty(adult))
        {
            node.adult = GetEnumValue<AdultScenes>(adult);
        }
        else
        {
            node.adult = AdultScenes.None;
        }

        dataList[ID].TryGetValue("NextType", out string next);
        if (!string.IsNullOrEmpty(next))
        {
            node.NextType = GetEnumValue<NodeNextType>(next);
        }

        dataList[ID].TryGetValue("Location", out string location);

        if (!string.IsNullOrEmpty(location))
        {
            node.location = GetEnumValue<Locations>(location);
        }
        else
        {
            node.location = Locations.None;
        }

        dataList[ID].TryGetValue("DayTime", out string time);

        if (!string.IsNullOrEmpty(time))
        {
            node.dayTime = GetEnumValue<DayTimeType>(time);
        }

        dataList[ID].TryGetValue("Event", out string _event);

        if (!string.IsNullOrEmpty(_event))
        {
            node.Event = GetEnumValue<Events>(_event);
        }

        dataList[ID].TryGetValue("EventID", out string _eventID);

        if (!string.IsNullOrEmpty(_eventID))
        {
            node.EventID = ParseInt(_eventID);
        }

        dataList[ID].TryGetValue("WheekDay", out string wd);

        if (!string.IsNullOrEmpty(wd))
        {
            node.isWheekSkip = true;
            node.wheekDay = GetEnumValue<WheekDay>(wd);
        }
        else
        {
            node.isWheekSkip = false;
        }

        dataList[ID].TryGetValue("SlideID", out string slide);
        if (!string.IsNullOrEmpty(slide))
        {
            node.slideID = ParseInt(slide);
        }

        dataList[ID].TryGetValue("IsClearSlides", out string isClear);

        if (!string.IsNullOrEmpty(isClear))
        {
            int c = ParseInt(isClear);

            node.isClear = c > 0;
        }

        dataList[ID].TryGetValue("BubbleID", out string bID);

        if (!string.IsNullOrEmpty(bID))
        {
            int b = ParseInt( bID);

            node.bubbleID = b;
        }

        nodes.Add(node);
    }

    public CNode GetNode(int ID) => nodes[ID];
    public Sprite GetSprite(int ID) => images[ID];

    public int GetLength => nodes.Count;
}

[System.Serializable]
public class CNode
{
    public List<StringLang> Texts = new();

    public NodeNextType NextType;
    public int slideID;
    public int bubbleID;
    public Locations location;
    public DayTimeType dayTime;
    public Events Event;
    public int EventID;
    public AdultScenes adult;
    public bool isClear;
    public bool isWheekSkip;
    public WheekDay wheekDay;

    public string GetText()
    {
        return Texts.Find(t => t.language == LocalizationManager.Language).Text;
    }
}
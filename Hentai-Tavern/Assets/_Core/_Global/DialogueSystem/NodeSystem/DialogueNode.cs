using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "GAME/Dialogues/DialogueNode")]
public class DialogueNode : CSVLoadConfig
{
    public Character character;
    public Locations location;
    public List<DNode> nodes = new();

    private DNode currentNode;

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
            dataList[ID].TryGetValue("Type", out string type);

            if (type == "dialogue")
            {
                CreateNode(dataList, ID);
            }
            else
            {
                CreateAnswer(dataList, ID);
            }
        }

        SetDirty(this);
    }

    private void CreateNode(List<Dictionary<string, string>> dataList, int ID)
    {
        DNode node = new DNode();
        dataList[ID].TryGetValue("Name", out string value);
        node.Name = GetEnumValue<Character>(value);

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

        dataList[ID].TryGetValue("Emotion", out string emotion);

        if (!string.IsNullOrEmpty(emotion))
        {
            node.emotion = GetEnumValue<Emotions>(emotion);
        }

        dataList[ID].TryGetValue("Suit", out string suit);

        if (!string.IsNullOrEmpty(suit))
        {
            node.suit = GetEnumValue<CharacterSuitType>(suit);
        }

        dataList[ID].TryGetValue("Event", out string _event);
        dataList[ID].TryGetValue("EventID", out string _eventID);

        if (!string.IsNullOrEmpty(_event))
        {
            node.Event = GetEnumValue<Events>(_event);
            node.EventID = ParseInt(_eventID);
        }

        dataList[ID].TryGetValue("NextType", out string next);
        if (!string.IsNullOrEmpty(next))
        {
            node.NextType = GetEnumValue<NodeNextType>(next);
            node.NextID = ParseInt(dataList, ID, "NextID");
        }

        currentNode = node;
        nodes.Add(node);
    }

    private void CreateAnswer(List<Dictionary<string, string>> dataList, int ID)
    {
        DAnswer answer = new();

        answer.Texts = SetLang(dataList[ID]);

        answer.NextID = ParseInt(dataList, ID, "NextID");
        currentNode.answers.Add(answer);
    }

    public DNode GetNode(int ID) => nodes[ID];

    public int GetLength => nodes.Count;
}



[System.Serializable]
public class DNode
{
    public  List<StringLang> Texts = new();

    public Character Name;
    public CharacterSuitType suit;
    public NodeNextType NextType;
    public int NextID;
    public Emotions emotion;
    public Events Event;
    public int EventID;
    public AdultScenes adult;
    public List<DAnswer> answers = new();
    public int datingCount = 0;

    public string GetText()
    {
        return Texts.Find(t => t.language == LocalizationManager.Language).Text;
    }
}

[System.Serializable]
public class DAnswer
{
    public List<StringLang> Texts = new();

    public int NextID;

    public string GetText()
    {
        return Texts.Find(t => t.language == LocalizationManager.Language).Text;
    }
}

public enum NodeNextType
{
    None,
    Next,
    End,
    EndTrue,
    EndToState,
    EndEvent
}

public enum RewardNodeType
{
    None,
    Reward
}

public enum DialogueSide
{
    Left,
    Right
}

public enum AnswerReq
{
    None,
    LoveMisha,
    LoveNaomy,
    LoveSamantha
}
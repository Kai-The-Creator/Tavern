using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using _Core._Global.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCharacter : Element
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI dialogueText, nameText;
    [SerializeField] private GameObject dialoguePopup;
    [SerializeField] private Shadow shadow;

    public RectTransform rectTransform;

    public Vector2 characterStartPos;
    public Vector2 characterEndPos;

    public Character currentCharacter { get; private set; }
    private CharacterSuitType currentSuit;

    public bool isSkiped { get; private set; }  
    private string textToSkip;

    private Vector3 currentScale;
    private Emotions curEmo;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        shadow = characterImage.GetComponent<Shadow>();
        currentScale = characterImage.transform.localScale;
        if(currentScale == Vector3.zero)
        {
            currentScale = Vector3.one;
        }
    }

    public void Init(DNode node)
    {

        Activate();
        /*CharacterConfig config = GameInjectContainer.instance.Characters.GetCharacter(node.Name);
        CharacterSuit suit = config.GetSuit(node.suit);

        if (node.Name != Character.None)
        {
            if (node.emotion != curEmo)
            {
                curEmo = node.emotion;
                //AudioSystem.instance.SetSound(config.GetEmotionSound(node.emotion));
            }
        }

        if (currentCharacter != node.Name)
        {
            

            currentCharacter = node.Name;

            nameText.text = GameInjectContainer.instance.Characters.GetCharacter(currentCharacter).GetName();

            currentSuit = node.suit;
            characterImage.sprite = suit.characterSprite;
            characterImage.sprite = suit.GetStage(node.characterSuitStage).GetEmotion(node.emotion).emotionSprite;

        }
        else
        {
            if(currentSuit != CharacterSuitType.Suite_1)
            {
                characterImage.sprite = suit.characterSprite;
                characterImage.sprite = suit.GetStage(node.characterSuitStage).GetEmotion(node.emotion).emotionSprite;
            }
        }

        
        characterImage.sprite = suit.GetStage(node.characterSuitStage).GetEmotion(node.emotion).emotionSprite;
        StartCoroutine(waitSpitchText(node.GetText()));*/
    }

    public override void Deactivate()
    {
        //animationPopup.PreviewStart();
        base.Deactivate();
    }

    public void Clear()
    {
        dialogueText.text = "";
        dialoguePopup.transform.DOScale(Vector3.zero, 0.3f);

        characterImage.transform.DOScale(currentScale, 0.3f);

        float shX = shadow.effectDistance.x;

        DOTween.To(() => shX, x =>
        {
            shadow.effectDistance = new Vector2(x, shadow.effectDistance.y);
        }, 0, 0.3f);

        //shadow.effectDistance = new Vector2(12f, -1);
    }

    private IEnumerator waitSpitchText(string text)
    {
        dialoguePopup.transform.localScale = Vector3.zero;
        dialoguePopup.transform.DOScale(Vector3.one, 0.3f);

        dialogueText.text = text;
        yield return null;
        characterImage.transform.DOScale(currentScale * 1.03f, 0.3f);
        float shX = shadow.effectDistance.x;

        if (shX < 25)
        {
            DOTween.To(() => shX, x =>
            {
                shadow.effectDistance = new Vector2(x, shadow.effectDistance.y);
            }, 25, 0.3f);
        }
       // shadow.effectDistance = new Vector2(25f, -1);
    }

    public void Skip()
    {
        StopAllCoroutines();

        dialogueText.text = textToSkip;

        isSkiped = true;
    }
}

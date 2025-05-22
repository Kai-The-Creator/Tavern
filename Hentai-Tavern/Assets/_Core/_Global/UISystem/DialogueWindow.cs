using System.Collections.Generic;
using _Core._Global.DialogueSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UISystem
{
    public class DialogueWindow : UIWindow
    {
        [Header("Элементы UI диалога")] [SerializeField]
        private TextMeshProUGUI mainText;

        [SerializeField] private Button nextButton;

        [Header("Ответы")] [SerializeField] private RectTransform answersRoot;
        [SerializeField] private DialogueAnswerButton answerButtonPrefab;

        private List<DialogueAnswerButton> _createdAnswerButtons = new List<DialogueAnswerButton>();

        [Header("Изображения диалога")] [SerializeField]
        private Image backgroundImage;

        [SerializeField] private Image characterImage;
        [SerializeField] private SkeletonGraphic skeletonGraphic;

        private IDialogueService _dialogueController;

        public void Init(IDialogueService controller)
        {
            _dialogueController = controller;

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(() =>
                {
                    if (_dialogueController != null)
                        _dialogueController.Next();
                });
            }
        }

        public override async UniTask Show(WindowLayer layer)
        {
            await base.Show(layer);
        }

        public override async UniTask Close()
        {
            ClearAnswers();
            await base.Close();
        }

        public void SetDialogueText(string text)
        {
            if (mainText != null) mainText.text = text;
        }

        public void SetBackground(Sprite bgSprite)
        {
            if (backgroundImage == null) return;

            if (bgSprite == null)
            {
                backgroundImage.gameObject.SetActive(false);
            }
            else
            {
                backgroundImage.gameObject.SetActive(true);
                backgroundImage.sprite = bgSprite;

                backgroundImage.DOFade(0f, 0f);
                backgroundImage.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
            }
        }

        public void SetCharacter(CharacterConfig config)
        {
            if (config == null) return;

            characterImage.gameObject.SetActive(config.spineView == null ? true : false);
            skeletonGraphic.gameObject.SetActive(config.spineView == null ? false : true);

            if (config.spineView == null)
            {
                characterImage.sprite = config.characterView;

                characterImage.transform.localScale = Vector3.zero;
                characterImage.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                skeletonGraphic.skeletonDataAsset = config.spineView;
                skeletonGraphic.Initialize(true);
            }
        }

        public void SetAnswers(List<DAnswer> answers)
        {
            if (answers == null || answers.Count == 0)
            {
                ClearAnswers();
                return;
            }

            if (answersRoot == null || answerButtonPrefab == null) return;

            ClearAnswers();

            foreach (var ans in answers)
            {
                var btn = Instantiate(answerButtonPrefab, answersRoot);
                btn.Init(ans.GetText(), () =>
                {
                    if (_dialogueController != null)
                    {
                        _dialogueController.SelectAnswer(ans);
                    }
                });

                _createdAnswerButtons.Add(btn);
            }
        }

        private void ClearAnswers()
        {
            foreach (var btn in _createdAnswerButtons)
            {
                if (btn != null)
                    Destroy(btn.gameObject);
            }

            _createdAnswerButtons.Clear();
        }
    }
}
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static Image fade;
    private static TweenerCore<Color, Color, ColorOptions> _fadeAinm;
    private static Material fadeMaterial;
    private static float duration;
    private static GameObject _text;
    private static float _dealyTime;
    private static GameObject bS;

    private static Action _actionAfter;
    private static bool _startFade = false;
    public GameScenes startScene;
    public Material material;
    public float _fadeDuration;
    public float delayTime;
    public GameObject text;
    public GameObject blockScreen;

    private void Start()
    {
        DontDestroyOnLoad(this);
        fade = GetComponentInChildren<Image>();
        fade.raycastTarget = false;
        fadeMaterial = material;
        duration = _fadeDuration;
        _dealyTime = delayTime;
        _text = text;
        bS = blockScreen;
        _text.SetActive(false);
        fadeMaterial.SetFloat("_CutHeight", 1);
        //StartCoroutine(waitToStart());
        blockScreen.SetActive(false);
    }

    public static void LoadScene(GameScenes scene)
    {
        Fade(scene, null);
    }

    public static void LoadScene(GameScenes scene, Action endEction)
    {
        Fade(scene, endEction);
    }

    private static void Fade(GameScenes scene, Action end)
    {
        bS.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        //fade.raycastTarget = true;
        sequence
            .AppendCallback(() => FadeIn(Color.black, () => SceneManager.LoadScene(scene.ToString()))).OnComplete(() =>
            {
                end?.Invoke();
            })
            .AppendInterval(1f)
            .AppendCallback(() => _text.SetActive(true))
            .AppendInterval(_dealyTime)
            .AppendCallback(() => FadeOut())
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _text.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _text.SetActive(false);
                bS.SetActive(false);
            });
    }

    public static void Fade(Action action, float time = 2)
    {
        bS.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        float t = time == 2 ? _dealyTime : time;
        sequence
            .AppendCallback(() => FadeIn(Color.black, () => action?.Invoke()))
            .AppendInterval(1f)
            .AppendCallback(() => _text.SetActive(true))
            .AppendInterval(t)
            .AppendCallback(() => FadeOut())
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _text.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _text.SetActive(false);
                bS.SetActive(false);
            });
    }

    public static void FadeIn(Color color, Action actionAfter = null)
    {
        _fadeAinm.Kill();
        //fade.raycastTarget = true;
        if (_startFade)
        {
            _actionAfter = actionAfter;

            fadeMaterial.SetFloat("_CutHeight", 1);
            DOTween.To(() => fadeMaterial.GetFloat("_CutHeight"), x => {
                fadeMaterial.SetFloat("_CutHeight", x);
            }, -1, duration).OnComplete(() =>
            {
                _actionAfter?.Invoke();
            });
        }
        else
        {
            _startFade = true;
            fadeMaterial.SetFloat("_CutHeight", 1);
            _actionAfter = actionAfter;
            //fade.raycastTarget = true;


            DOTween.To(() => fadeMaterial.GetFloat("_CutHeight"), x => {
                fadeMaterial.SetFloat("_CutHeight", x);
            }, -1, duration).OnComplete(() =>
            {
                _startFade = false;
                _actionAfter?.Invoke();
            });
        }
    }
    private static void FadeOut()
    {
        fadeMaterial.SetFloat("_CutHeight", -1);

        DOTween.To(() => fadeMaterial.GetFloat("_CutHeight"), x => {
            fadeMaterial.SetFloat("_CutHeight", x);
        }, 1, duration).OnComplete(() =>
        {
            fade.raycastTarget = false;
        });
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator waitToStart()
    {
        yield return new WaitForSeconds(3f);
        LoadScene(startScene);
    }
}
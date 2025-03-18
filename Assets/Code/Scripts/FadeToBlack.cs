using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FadeToBlack : MonoBehaviour
{
    private Image image;
    private VideoPlayer videoPlayer;
    [SerializeField] private bool fadeOutOnStart = true;
    
    private Color black = Color.black;
    private Color clear;

    private void Start()
    {
        clear = new Color(black.r, black.g, black.b, 0);
        image = GetComponentInChildren<Image>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        if (fadeOutOnStart)
        {
            DoFadeOut();
        }
    }

    public void DoFadeIn(float duration = 1f)
    {
        image.color = clear;
        image.DOFade(1f, duration);
    }

    public void DoFadeOut(float duration = 1f)
    {
        image.color = Color.black;
        image.DOFade(0f, duration);
    }

    public void Toggle()
    {
        image.enabled = !image.enabled;
    }

    public float PlayVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            
        }

        return (float) videoPlayer.clip.length;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            videoPlayer.Play();
        }
    }
}

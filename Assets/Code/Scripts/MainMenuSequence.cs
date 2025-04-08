using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSequence : MonoBehaviour
{
    [SerializeField] private CinemachineCamera startCamera;
    [SerializeField] private CinemachineCamera endCamera;
    [SerializeField] private FadeToBlack fadeToBlack;

    public void Play(string sceneName)
    {
        endCamera.enabled = true;
        StartCoroutine(LoadLevel(sceneName));
        
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        yield return new WaitForSeconds(2f);
     
        fadeToBlack.DoFadeIn(2f);
        
        yield return new WaitForSeconds(2f);
        
        ShowIntroduction();
        
    }

    private void ShowIntroduction()
    {
        //Debug.Log("Introduction goes here");
        // For now, just load the level for the demo

        LevelManager.Instance.LoadNextLevel();
    }
    
    
}

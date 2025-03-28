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
        
        yield return new WaitForSeconds(1f);
        
        fadeToBlack.DoFadeIn();
        
        SceneManager.LoadScene("CharacterSelection");
        
        // fadeToBlack.Toggle();
        //
        // float seconds = fadeToBlack.PlayVideo();
        //
        // yield return new WaitForSeconds(seconds);
        //
        // fadeToBlack.Toggle();
        //
        
        
    }

    private void ShowCharacterSelection()
    {
        
    }
    
    
}

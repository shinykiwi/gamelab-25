using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSequence : MonoBehaviour
{
    [SerializeField] private CinemachineCamera startCamera;
    [SerializeField] private CinemachineCamera endCamera;

    public void Play(string sceneName)
    {
        endCamera.enabled = true;
        StartCoroutine(LoadLevel(sceneName));
        
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(sceneName);
        
    }
}

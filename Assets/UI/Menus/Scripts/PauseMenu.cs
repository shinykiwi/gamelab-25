using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
   private MenuAudio menuAudio;

   [SerializeField] private GameObject options;
   [SerializeField] private GameObject pause;
   [SerializeField] private GameObject controls;
   [SerializeField] private GameObject parent;

   private void Start()
   {
      menuAudio = GetComponentInChildren<MenuAudio>();
      
      pause.SetActive(false);
      controls.SetActive(false);
      options.SetActive(false);
      parent.SetActive(false);
   }

   private void Toggle()
   {
      parent.SetActive(!parent.activeSelf);
   }
   
   public void OnResumeButtonClick()
   {
      Toggle();
      menuAudio.PlayClickSound();
   }

   public void OnOptionsButtonClick()
   {
      menuAudio.PlayClickSound();
      
      // Show the options panel
      options.SetActive(true);
      
      // Hide the pause panel
      pause.SetActive(false);
      controls.SetActive(false);
   }

   public void OnBackButtonClick()
   {
      menuAudio.PlayBackSound();
      
      // Show the pause panel
      pause.SetActive(true);
      
      // Hide the other panels
      options.SetActive(false);
      controls.SetActive(false);
   }

   public void OnControlsMenuClick()
   {
      menuAudio.PlayClickSound();
      
      // Show the controls menu
      controls.SetActive(true);
      
      // Hide the other menus
      pause.SetActive(false);
      options.SetActive(false);
      
   }

   public void OnRestartButtonClick()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void OnQuitButtonClick()
   {
      menuAudio.PlayBackSound();
      Application.Quit();
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         if (pause.activeSelf)
         {
            Toggle();
         }
         else
         {
            if (!parent.activeSelf)
            {
               // Show the pause panel
               Toggle();
               pause.SetActive(true);
            }
            else
            {
               OnBackButtonClick();
            }
         }
         
      }
   }
}

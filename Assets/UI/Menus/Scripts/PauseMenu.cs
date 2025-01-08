using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
   private Canvas canvas;
   private MenuAudio audio;

   [SerializeField] private GameObject options;
   [SerializeField] private GameObject pause;
   [SerializeField] private GameObject controls;

   private void Awake()
   {
      canvas = GetComponent<Canvas>();
      audio = GetComponentInChildren<MenuAudio>();
      
      // Hide other menus to start
      options.SetActive(false);
      controls.SetActive(false);
   }

   private void Toggle()
   {
      canvas.enabled = !canvas.enabled;
   }
   
   public void OnResumeButtonClick()
   {
      Toggle();
      audio.PlayClickSound();
   }

   public void OnOptionsButtonClick()
   {
      audio.PlayClickSound();
      
      // Show the options panel
      options.SetActive(true);
      
      // Hide the pause panel
      pause.SetActive(false);
      controls.SetActive(false);
   }

   public void OnBackButtonClick()
   {
      audio.PlayBackSound();
      
      // Show the pause panel
      pause.SetActive(true);
      
      // Hide the other panels
      options.SetActive(false);
      controls.SetActive(false);
   }

   public void OnControlsMenuClick()
   {
      audio.PlayClickSound();
      
      // Show the controls menu
      controls.SetActive(true);
      
      // Hide the other menus
      pause.SetActive(false);
      options.SetActive(false);
      
   }

   public void OnQuitButtonClick()
   {
      audio.PlayBackSound();
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
            OnBackButtonClick();
         }
         
      }
   }
}

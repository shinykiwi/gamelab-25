using Code.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Buttons
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    // Pages
    [Header("Pages")]
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject menu;

    [Header("Pages Selectables")]
    [SerializeField] private Selectable creditsFirstSelectable;
    [SerializeField] private Selectable optionsFirstSelectable;
    [SerializeField] private Selectable menuFirstSelectable;

    // Main menu config
    [Header("Settings")]
    [Tooltip("Scene to load upon play, if any. Will hide the menu instead if no scene asset.")]
    [SerializeField] private string scene;
    [SerializeField] private MainMenuSequence sequence;

    private MenuAudio menuAudio;

    // Main menu itself
    private Canvas canvas;

    private InputAction[] cancelActions;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuAudio = GetComponentInChildren<MenuAudio>();
        
        // Hide the credits menu and the options menu to start with
        //credits.SetActive(false);
        //options.SetActive(false);

        EventSystem.current.SetSelectedGameObject(playButton.gameObject);

        var playerInputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None).ToList();
        cancelActions = playerInputs.ConvertAll(playerInput => playerInput.actions["Cancel"]).ToArray();
    }

    private void Update()
    {
        // If the player is in a menu (other than main menu)
        if (credits.activeSelf || options.activeSelf)
        {
            // If the escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape) || cancelActions.Any(action => action.triggered))
            {
                OnBackButton(); // do the same thing as if the back button was clicked
            }
        }
    }

    /// <summary>
    /// Hides the MainMenu.
    /// </summary>
    private void Hide()
    {
        canvas.enabled = false;
    }

    /// <summary>
    /// Shows the MainMenu.
    /// </summary>
    private void Show()
    {
        canvas.enabled = true;
    }
    
    /// <summary>
    /// Executes when the play button is clicked, loads the specified scene (if any).
    /// </summary>
    public void OnPlayButton()
    {
        menuAudio.PlayClickSound();
        if (scene != null)
        {
            sequence.Play(scene);
            Hide();
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// Called by the credits button in the UI.
    /// </summary>
    public void OnCreditsButton()
    {
        menuAudio.PlayClickSound();
        
        // Show the credits menu
        credits.SetActive(true);
        
        // Hide the options and credits menu
        options.SetActive(false);
        menu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(creditsFirstSelectable.gameObject);
    }

    /// <summary>
    /// Called be the options button in the UI.
    /// </summary>
    public void OnOptionsButton()
    {
        menuAudio.PlayClickSound();
        
        // Show the options menu
        options.SetActive(true);
        
        // Hide the credits and main menu
        credits.SetActive(false);
        menu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(optionsFirstSelectable.gameObject);
    }

    /// <summary>
    /// Called by the quit button in the UI, quits the game in a build version.
    /// </summary>
    public void OnQuitButton()
    {
        menuAudio.PlayBackSound();
        
        // Quits the game
        Application.Quit();
    }

    /// <summary>
    /// Hides every menu except for the main menu.
    /// </summary>
    private void HideAllButMain()
    {
        credits.SetActive(false);
        options.SetActive(false);
        menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(menuFirstSelectable.gameObject);
    }

    /// <summary>
    /// Called when any back button is pressed in the UI.
    /// </summary>
    public void OnBackButton()
    {
        menuAudio.PlayBackSound();
        HideAllButMain();
    }
}

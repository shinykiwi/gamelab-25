using Code.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private MenuAudio menuAudio;

    [SerializeField] private GameObject options;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject parent;

    // Objects that are selected when their menu is opened
    [SerializeField] private GameObject pauseSelectable;
    [SerializeField] private GameObject optionsSelectable;
    [SerializeField] private GameObject controlsSelectable;

    private InputAction[] pauseActions;
    private InputAction[] cancelActions;

    private void Start()
    {
        menuAudio = GetComponentInChildren<MenuAudio>();

        var playerInputs = Level.Instance.players.ConvertAll(player => player.GetComponent<PlayerInput>());
        pauseActions = playerInputs.ConvertAll(playerInput => playerInput.actions["Pause"]).ToArray();
        cancelActions = playerInputs.ConvertAll(playerInput => playerInput.actions["Cancel"]).ToArray();

        pause.SetActive(false);
        controls.SetActive(false);
        options.SetActive(false);
        parent.SetActive(false);
    }

    private void Toggle()
    {
        parent.SetActive(!parent.activeSelf);

        var playerInputs = Level.Instance.players.ConvertAll(player => player.GetComponent<PlayerInput>());
        if(parent.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(pauseSelectable);
            LevelManager.Instance.Pause();
        }
        else
        {
            LevelManager.Instance.Resume();
        }
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

        EventSystem.current.SetSelectedGameObject(optionsSelectable);
    }

    public void OnBackButtonClick()
    {
        menuAudio.PlayBackSound();

        // Show the pause panel
        pause.SetActive(true);

        // Hide the other panels
        options.SetActive(false);
        controls.SetActive(false);

        EventSystem.current.SetSelectedGameObject(pauseSelectable);
    }

    public void OnControlsMenuClick()
    {
        menuAudio.PlayClickSound();

        // Show the controls menu
        controls.SetActive(true);

        // Hide the other menus
        pause.SetActive(false);
        options.SetActive(false);

        EventSystem.current.SetSelectedGameObject(controlsSelectable);
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButtonClick()
    {
        menuAudio.PlayBackSound();
        LevelManager.Instance.LoadDefault();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || pauseActions.Any(action => action.triggered))
        {
            if(pause.activeSelf)
            {
                Toggle();
            }
            else
            {
                if(!parent.activeSelf)
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

        if(cancelActions.Any(action => action.triggered) 
            && !pause.activeSelf 
            && parent.activeSelf)
        {
            OnBackButtonClick();
        }
    }
}

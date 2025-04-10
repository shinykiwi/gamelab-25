using Code.Scripts;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private int currentLevel = 0;
    private FadeToBlack fadeToBlack;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this; 
        }
        
        // Prevents Unity from deleting this object when a new scene is loaded.
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically find references when the scene is loaded
        FindFade();
        
        if (fadeToBlack == null)
            Debug.LogWarning("FadeCanvas reference not found in scene!");

        // Make sure the game is not paused when a new scene is loaded
        Resume();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid memory leaks
    }

    private void Start()
    {
        FindFade();
        string[] sceneName = SceneManager.GetActiveScene().name.Split("_");
        if (sceneName[0] == "Level")
        {
            currentLevel = int.Parse(sceneName[1]);
        }
    }

    private void FindFade()
    {
        fadeToBlack = FindFirstObjectByType<FadeToBlack>();
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        string sceneName = "Level_";
        sceneName += level.ToString();
        
        Debug.Log("Loading " + sceneName + "...");
        
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch
        {
            Debug.Log("Could not load scene " + sceneName + " because it doesn't exist.");
        }
        
    }

    public void LoadDefault()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Pause()
    {
        Level.Instance?.players.ForEach(player => player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI"));

        Time.timeScale = 0;
    }

    public void Resume()
    {
        Level.Instance?.players.ForEach(player => player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player"));

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Load level 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(1);
        }
        
        // Load level 2
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           LoadLevel(2);
        }
        
        // Load level 3
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLevel(3);
        }
        
        // Load level 4
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
           LoadLevel(4);
        }
        
        // Load level 5
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadLevel(5);
        }
        
        // Load level 6
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LoadLevel(6);
        }
        
        // Load level 7
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LoadLevel(7);
        }
        
        
        
    }
}

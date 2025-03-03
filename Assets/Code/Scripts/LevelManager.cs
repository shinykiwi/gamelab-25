using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private int currentLevel = 1;
    private FadeToBlack fadeToBlack;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        
        // Prevents Unity from deleting this object when a new scene is loaded.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeToBlack = FindFirstObjectByType<FadeToBlack>();
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        LoadLevel(currentLevel);
    }

    private void LoadLevel(int level)
    {
        string sceneName = "Level_";
        sceneName += level.ToString();
        
        Debug.Log("Loading " + sceneName + "...");
        
        fadeToBlack.DoFadeIn();
        
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.Log("Could not load scene " + sceneName + " because it doesn't exist.");
        }
        
    }

    public void LoadDefault()
    {
        SceneManager.LoadScene("MainMenuScene");
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
        
    }
}

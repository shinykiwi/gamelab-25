using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int currentLevel = 1;
    
    public void LoadNextLevel()
    {
        currentLevel++;
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        string sceneName = "Level_";
        sceneName += level.ToString();
        SceneManager.LoadScene(sceneName);
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

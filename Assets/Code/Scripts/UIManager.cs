using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Spawn the pause menu on start
        pauseMenu = Instantiate(pauseMenu, transform);
    }
}
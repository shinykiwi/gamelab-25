using System;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    private GameObject theVoid;
    private void Update()
    {
        // If you press 0 on keyboard
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!theVoid)
            {
                theVoid = GameObject.FindGameObjectWithTag("Void");
            }

            theVoid.tag = theVoid.CompareTag("Void") ? "Untagged" : "Void";
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LevelTrigger trigger = FindFirstObjectByType<LevelTrigger>();
            trigger?.Show();
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LevelManager.Instance.LoadDefault();
        }
    }
}

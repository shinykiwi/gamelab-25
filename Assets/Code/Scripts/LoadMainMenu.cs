using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{
    void Start()
    {
        LevelManager.Instance.LoadDefault();
    }
}

using UnityEngine;

public class GoToCreditsTag : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

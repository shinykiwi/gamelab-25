using UnityEngine;
using UnityEngine.Events;

public class TimerUtil : MonoBehaviour
{
    [SerializeField]
    float callAfterSec = 1.0f;

    [SerializeField]
    UnityEvent callEvent = null;

    float timer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= callAfterSec)
        {
            callEvent?.Invoke();
        }
    }
}

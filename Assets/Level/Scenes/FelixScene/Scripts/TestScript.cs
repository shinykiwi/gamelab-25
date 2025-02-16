using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    PlayerInput obj;

    private void Start()
    {
        var t = FindFirstObjectByType<PlayerInputManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            FindFirstObjectByType<PlayerDeviceManager>().AssignGamepad(obj);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            FindFirstObjectByType<PlayerDeviceManager>().AssignKeyboard(obj);
        }
    }
}

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeviceManager : MonoBehaviour
{
    [SerializeField]
    PlayerInput playerInput1;
    [SerializeField]
    PlayerInput playerInput2;

    private void Start()
    {
        AssignInitialDevices();
    }

    public void AssignGamepad(PlayerInput playerInput)
    {
        PlayerInput otherPlayerInput = playerInput == playerInput1 ? playerInput2 : playerInput1;

        var devices = InputSystem.devices;
        Gamepad gamepad = devices.FirstOrDefault(d => d is Gamepad
            && (!playerInput1.user.valid || !playerInput1.devices.Contains(d))) as Gamepad;
        if(gamepad != null)
        {
            playerInput.SwitchCurrentControlScheme(gamepad);
        }
    }

    public void AssignKeyboard(PlayerInput playerInput)
    {
        PlayerInput otherPlayerInput = playerInput == playerInput1 ? playerInput2 : playerInput1;
        if(otherPlayerInput.currentControlScheme != "Keyboard&Mouse")
        {
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
        else
        {
            playerInput.SwitchCurrentControlScheme("Keyboard2", Keyboard.current, Mouse.current);
        }
    }

    void AssignInitialDevices()
    {
        PlayerInput[] inputs = { playerInput1, playerInput2 };

        // Reset inputs
        foreach(var input in inputs)
        {
            // temporarily disable input to prevent devices from being automatically assigned
            input.enabled = false; 
            input.SwitchCurrentControlScheme();
            if(input.user.valid)
            {
                input.user.UnpairDevices();
            }
        }

        // Assign devices
        foreach(PlayerInput input in inputs)
        {
            input.enabled = true;
            if(!input.user.valid)
                continue;
            AssignGamepad(input); // Prefer Gamepads
            if(input.currentControlScheme != "Gamepad")
            {
                AssignKeyboard(input);
            }
        }
    }
}

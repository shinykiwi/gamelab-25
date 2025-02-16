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
        playerInput1.enabled = false;
        playerInput2.enabled = false;

        // Assign devices to players
        playerInput2.SwitchCurrentControlScheme();
        playerInput1.SwitchCurrentControlScheme();
        if(playerInput2.user.valid)
        {
            playerInput2.user.UnpairDevices();
        }
        if(playerInput1.user.valid)
        {
            playerInput1.user.UnpairDevices();
        }

        AssignDevice(playerInput1, playerInput2);
        AssignDevice(playerInput2, playerInput1);

    }

    void AssignDevice(PlayerInput playerInput, PlayerInput otherPlayerInput)
    {
        playerInput.enabled = true;

        var devices = InputSystem.devices;

        // Prioritize Gamepads
        Gamepad gamepad = devices.FirstOrDefault(d => d is Gamepad 
            && (!otherPlayerInput.user.valid || !otherPlayerInput.devices.Contains(d))) as Gamepad;
        if(gamepad != null)
        {
            playerInput.SwitchCurrentControlScheme(gamepad);
        }
        else if(otherPlayerInput.currentControlScheme != "Keyboard&Mouse") // Fallback to keyboard1
        {
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
        else // Fallback to keyboard2
        {
            playerInput.SwitchCurrentControlScheme("Keyboard2", Keyboard.current, Mouse.current);
        }
    }
}

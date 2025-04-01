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
        if(playerInput1 == null || playerInput2 == null)
        {
            Debug.LogError("Missing PlayerInput references.", this);
        }
        AssignInitialDevices();
    }

    public void AssignGamepad(PlayerInput playerInput)
    {
        PlayerInput otherPlayerInput = playerInput == playerInput1 ? playerInput2 : playerInput1;

        var devices = InputSystem.devices;
        Gamepad gamepad = devices.FirstOrDefault(d => d is Gamepad
            && (!otherPlayerInput.user.valid || !otherPlayerInput.devices.Contains(d))) as Gamepad;
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
            // Reassign both players to Keyboard only
            ResetAndDisableInputs();
            otherPlayerInput.enabled = true;
            otherPlayerInput.SwitchCurrentControlScheme("Keyboard1", Keyboard.current, Mouse.current);
            playerInput.enabled = true;
            playerInput.SwitchCurrentControlScheme("Keyboard2", Keyboard.current, Mouse.current);
        }
    }


    void ResetAndDisableInputs()
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
    }

    void AssignInitialDevices()
    {
        PlayerInput[] inputs = { playerInput1, playerInput2 };

        ResetAndDisableInputs();

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

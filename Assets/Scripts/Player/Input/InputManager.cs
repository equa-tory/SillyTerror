using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput input;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public bool i_sprint;
    public bool i_jump;


    //--------------------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInput();

            input.Main.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            input.Main.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

            input.Action.Sprint.performed += ctx => i_sprint = true;
            input.Action.Sprint.canceled += ctx => i_sprint = false;

            input.Action.Jump.performed += ctx => i_jump = true;
            input.Action.Jump.canceled += ctx => i_jump = false;

        }

        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
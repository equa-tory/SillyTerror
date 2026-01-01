//by TheSuspect
//19.10.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private InputManager input;
    [SerializeField] private Transform camPos;


    [Header("Variables")]
    [SerializeField] private float sensX = 250f;
    [SerializeField] private float sensY = 250f;

    private float xRotation;
    private float yRotation;

    [SerializeField] private float rotationClamp = 90;
    [SerializeField] private bool showCursor;


    //--------------------------------------------------------------------------------------------

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (showCursor) return;

        MouseInput();
        transform.position = camPos.position;
    }

    private void MouseInput()
    {
        //mouse input
        // float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        // float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        float mouseX = input.cameraInput.x * Time.deltaTime * sensX;
        float mouseY = input.cameraInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -rotationClamp, rotationClamp);

        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerTransform.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}

//by TheSuspect
//19.10.2023

using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;


    [Header("Movement")]
    [SerializeField] private float speedChangeRate = 1f;
    private float currentSpeed;
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float sprintSpeed = 12f;

    private Vector3 moveDir;

    [SerializeField] private float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool extitingSlope;


    [Header("Jumping")]
    private float airMultiplayer = 1f;
    private float groundDrag = 5f;
    [SerializeField] private float jumpForce = 0f;
    private float jumpCooldwn = 0.75f;
    private bool readyToJump = true;

    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;


    [Header("Statements")]
    private MovementState state;
    private enum MovementState
    {
        walking,
        sprinting,
        air
    }
    private bool isSprinting;


    [Header("Input")]
    [SerializeField] private InputManager input;


    [Header("Audio")]
    [SerializeField] private List<AudioSource> stepSounds = new List<AudioSource>();
    private float stepTimer = 0f;
    [SerializeField] private float stepTimeMax = 0.2f;


    //--------------------------------------------------------------------------------------------

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        StateHandler();
        MyInput();
        SpeedControl();
        GroundCheck();

        //Anti Falloff
        if (transform.position.y < -25f)
        {
            transform.position = new Vector3(transform.position.x, 15, transform.position.z);
            rb.velocity = Vector3.zero;
        }

        if (stepTimer > 0f) stepTimer -= Time.deltaTime;
        if (stepTimer > stepTimeMax) stepTimer = stepTimeMax;

        if (rb.velocity != Vector3.zero && stepTimer <= 0f && grounded)
        {
            AudioSource stepSource = Instantiate(stepSounds[Random.Range(0, stepSounds.Count)], transform);
            stepSource.volume = Random.Range(0.7f, 1f);
            stepSource.pitch = Random.Range(0.9f, 1.1f);
            stepSource.Play();
            Destroy(stepSource.gameObject, stepSource.clip.length);
            stepTimer = stepTimeMax / (currentSpeed / 1.5f);
        }
    }

    private void FixedUpdate() => Movement();

    //--------------------------------------------------------------------------------------------------------------------------------

    #region Movement
    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);

        #region Ground Drag
        if ((state == MovementState.walking || state == MovementState.sprinting) && grounded) rb.drag = groundDrag;
        else rb.drag = 0;
        #endregion
    }

    private void MyInput()
    {
        isSprinting = input.i_sprint;
        moveDir.x = input.movementInput.x;
        moveDir.z = input.movementInput.y;

        if (input.i_jump && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldwn);
        }
    }

    private void StateHandler()
    {
        if (isSprinting)
        {
            state = MovementState.sprinting;
            // currentSpeed = sprintSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, speedChangeRate * Time.deltaTime);
        }
        else if (grounded)
        {
            state = MovementState.walking;
            // currentSpeed = walkSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, 2 * speedChangeRate * Time.deltaTime);
        }
        else
        {
            state = MovementState.air;
            // currentSpeed = walkSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, 2 * speedChangeRate * Time.deltaTime);
        }
    }

    private void Movement()
    {
        moveDir = transform.forward * input.movementInput.y + transform.right * input.movementInput.x;

        if (OnSlope() && !extitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDir) * currentSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Realistic Air Speed
        if (currentSpeed > 0) airMultiplayer = 2 / currentSpeed;
        //

        if (grounded)
            rb.AddForce(moveDir.normalized * currentSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDir.normalized * currentSpeed * 10f * airMultiplayer, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (currentSpeed < 0) currentSpeed = 0;

        if (OnSlope() && !extitingSlope)
        {
            if (rb.velocity.magnitude > currentSpeed)
                rb.velocity = rb.velocity.normalized * currentSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > currentSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * currentSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    private void Jump()
    {
        extitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        extitingSlope = false;
    }

    private bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    #endregion
}

using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 8f;
    public float crouchMoveSpeed;
    private float normalMoveSpeed;
    public float sprintMoveSpeed;

    private Vector3 movement;

    [Header("Jump")]
    public int maxJumpCount = 2;
    private int jumpCount = 0;

    public float jumpForce = 5f;
    public float jumpCooldown = 0.5f;

    private bool isJumping = false;
    private bool canJump = true;

    [Header("Dash")]
    public int maxDashCount = 2;
    private int dashCount = 0;
    public float dashCooldown = 0.8f;

    public float dashDistance = 5f;
    public float dashSpeed = 50f;

    private bool isDashing = false;

    [Header("Physics")]
    private float gravity = -9.81f;
    private float groundDistance = 0.1f;
    public LayerMask groundMask;
    private bool isGrounded = false;
    private Vector3 velocity;

    [Header("Model")]
    [SerializeField] private GameObject model;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerInput();
        MovePlayer();
        ApplyGravity();
        RotateModel();
    }
    private void MovePlayer()
    {
        if (isDashing)
        {
            controller.Move(movement * dashSpeed * Time.deltaTime);
        }
        else
        {
            movement.Normalize();
            movement *= movementSpeed * Time.deltaTime;
            controller.Move(movement);
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void PlayerInput()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        movement = transform.TransformDirection(movement);
        movement.y = 0f;


        if (isGrounded)
        {
            jumpCount = 0;
            isJumping = false;
            canJump = true;
        }

        if (canJump && Input.GetButtonDown("Jump"))
        {
            canJump = false;
            ++jumpCount;
            StartCoroutine(JumpCoroutine());
        }

        if (Input.GetButtonDown("Fire3"))
        {
            if (!isDashing && dashCount <= maxDashCount)
            {
                dashCount++;
                StartCoroutine(DashCoroutine());
                StartCoroutine(ResetDashCount());
            }
        }
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1f, 0f), groundDistance, groundMask);
    }

    private void RotateModel()
    {
        model.transform.rotation = Quaternion.LookRotation(movement);
    }

    private IEnumerator JumpCoroutine()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        isJumping = true;

        yield return new WaitForSeconds(jumpCooldown);

        //canJump = true;
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + movement * dashDistance;
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);

        while (Time.time < startTime + journeyLength / dashSpeed)
        {
            velocity.y = 0; //Turn off gravity while dashing
            float distanceCovered = (Time.time - startTime) * dashSpeed;
            float journeyProgress = distanceCovered / journeyLength;
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, journeyProgress);
            controller.Move(newPosition - transform.position);
            yield return null;
        }

        isDashing = false;
    }

    private IEnumerator ResetDashCount()
    {  
        yield return new WaitUntil(() => isGrounded);
        yield return new WaitForSeconds(dashCooldown);
        dashCount--;
    }
}

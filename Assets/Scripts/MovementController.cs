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
    public float jumpForce = 5f;
    public float jumpCooldown = 0.5f;
    public int maxJumpCount = 1;
    private int jumpCount = 0;
    private bool isJumping = false;
    private bool canJump = true;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashSpeed = 50f;
    private bool isDashing = false;

    [Header("Physics")]
    public float gravity = -9.81f;
    public float groundDistance = 0.1f;
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
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
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
        else if (!isGrounded && jumpCount >= maxJumpCount)
        {
            
        }

        if (canJump && Input.GetButtonDown("Jump"))
        {
            canJump = false;
            ++jumpCount;
            StartCoroutine(JumpCoroutine());
        }

        if (Input.GetButtonDown("Fire3"))
        {
            if (!isDashing)
            {
                StartCoroutine(DashCoroutine());
            }
        }
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.9f, 0f), groundDistance, groundMask);
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
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 8f;
    public float crouchMoveSpeed;
    private float normalMoveSpeed;
    public float sprintMoveSpeed;

    public float dashDistance = 5f; 
    public float dashSpeed = 25f;

    private bool isDashing;
    private Vector3 dashDirection;


    public float GroundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplayer;
    public float crouchCooldown;

    bool readyToCrouch;
    bool readyToJump;
    bool isCrouching;
    bool isSprinting;

    float horizontal;
    float vertical;
    Vector3 moveDirection = Vector3.zero;

    private CharacterController controller;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {   
        PlayerInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MovePlayer() 
    {
        rb.AddForce(moveDirection.normalized * movementSpeed, ForceMode.Force);
    }

    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Dashing()
    {
        if (isDashing)
        {
            rb.AddForce(dashDirection * dashSpeed * Time.deltaTime);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //limit of velocity  if needed
        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void PlayerInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontal, 0f, 0f);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonDown("Fire3"))
        {
            if (!isDashing)
            {
                // Определяем направление рывка на основе направления взгляда персонажа
                dashDirection = moveDirection;

                // Запускаем корутину для выполнения рывка
                StartCoroutine(DashCoroutine());
            }
        }
    }
    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        // Вычисляем конечную точку рывка
        Vector3 targetPosition = transform.position + dashDirection * dashDistance;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Применяем перемещение в направлении рывка с определенной скоростью
            Dashing();

            yield return null;
        }
        isDashing = false;
    }
}

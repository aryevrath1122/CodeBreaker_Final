using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float maxJumpForce = 15f;
    public float minJumpForce = 5f;
    public float chargeRate = 10f;
    public float minDashSpeed = 10f;
    public float maxDashSpeed = 30f;
    public float dashChargeRate = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public CinemachineImpulseSource impulseSource;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isGrounded;
    private bool isDashing;
    private bool isChargingJump;
    private bool isChargingDash;
    private float currentJumpForce;
    private float currentDashSpeed;
    private float dashTimeLeft;
    private float lastDashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentJumpForce = minJumpForce;
        currentDashSpeed = minDashSpeed;

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    void HandleMovement()
    {
        if (isDashing || isChargingJump) return; // Skip movement if dashing or charging jump

        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            if (!isChargingJump)
            {
                isChargingJump = true;
                currentJumpForce = minJumpForce;
                animator.SetBool("isCharging", true); // Start charging animation
            }

            currentJumpForce += chargeRate * Time.deltaTime;
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);
        }

        if (isChargingJump && Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
            isChargingJump = false;
            animator.SetBool("isCharging", false); // Stop charging animation
            animator.SetTrigger("Jump");

            // Trigger screen shake only if the jump is fully charged
            if (currentJumpForce >= maxJumpForce && impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }
        }
    }

    void HandleDash()
    {
        if (isChargingJump || isDashing) return; // Skip dash handling if charging jump or already dashing

        if (isChargingDash)
        {
            currentDashSpeed += dashChargeRate * Time.deltaTime;
            currentDashSpeed = Mathf.Clamp(currentDashSpeed, minDashSpeed, maxDashSpeed);
        }

        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                dashTimeLeft -= Time.deltaTime;
                rb.velocity = new Vector2(currentDashSpeed * (spriteRenderer.flipX ? -1 : 1), rb.velocity.y);
            }
            else
            {
                isDashing = false;
                currentDashSpeed = minDashSpeed;
                animator.SetBool("isDashing", false);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            if (!isChargingDash)
            {
                isChargingDash = true;
                currentDashSpeed = minDashSpeed;
            }
        }

        if (isChargingDash && Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDashTime = Time.time;
            isChargingDash = false;
            animator.SetBool("isDashing", true);
        }
    }
}
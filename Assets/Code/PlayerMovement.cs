using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;               // Movement speed
    public float maxJumpForce = 15f;       // Maximum force applied when jumping
    public float minJumpForce = 5f;        // Minimum force applied when jumping
    public float chargeRate = 10f;         // Rate at which jump force increases
    public float minDashSpeed = 10f;       // Minimum speed during dash
    public float maxDashSpeed = 30f;       // Maximum speed during charged dash
    public float dashChargeRate = 20f;     // Rate at which dash speed increases
    public float dashTime = 0.2f;          // Duration of the dash
    public float dashCooldown = 1f;        // Cooldown time between dashes
    public LayerMask groundLayer;          // Layer that defines what is ground
    public Transform groundCheck;          // Position to check if the player is grounded

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
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
        currentJumpForce = minJumpForce;
        currentDashSpeed = minDashSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    void HandleMovement()
    {
        if (isDashing) return; // Skip regular movement if dashing

        // Get horizontal input
        float moveInput = Input.GetAxis("Horizontal");

        // Apply horizontal movement
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Flip the sprite based on movement direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;  // Face left
        }

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            if (!isChargingJump)
            {
                isChargingJump = true;
                currentJumpForce = minJumpForce; // Start charging from the minimum jump force
            }

            // Increase the jump force while the button is held, but don't exceed the maximum jump force
            currentJumpForce += chargeRate * Time.deltaTime;
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);
        }

        // Execute the jump when the button is released
        if (isChargingJump && Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
            isChargingJump = false; // Reset charging state
        }
    }

    void HandleDash()
    {
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
                currentDashSpeed = minDashSpeed; // Reset dash speed after dashing
            }
        }

        // Start charging dash when Left Shift is held
        if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            if (!isChargingDash)
            {
                isChargingDash = true;
                currentDashSpeed = minDashSpeed; // Start charging from the minimum dash speed
            }
        }

        // Execute dash when Left Shift is released
        if (isChargingDash && Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDashTime = Time.time;
            isChargingDash = false; // Reset charging state
        }
    }

}

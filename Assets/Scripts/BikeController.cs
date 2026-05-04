using UnityEngine;

/// <summary>
/// BikeController handles 2D arcade-style physics movement using Rigidbody2D.
/// Provides smooth acceleration, deceleration, and braking.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BikeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Maximum speed the bike can reach")]
    public float maxSpeed = 12f;
    [Tooltip("How quickly the bike accelerates")]
    public float acceleration = 15f;
    [Tooltip("Natural deceleration when no input is given")]
    public float deceleration = 8f;
    [Tooltip("Extra deceleration when braking (S key)")]
    public float brakeForce = 25f;
    [Tooltip("How fast the bike turns around")]
    public float turnSpeed = 10f;

    [Header("Physics")]
    [Tooltip("Linear drag applied by Rigidbody2D")]
    public float linearDrag = 2f;

    // Internal
    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;

    /// <summary>
    /// Current speed magnitude (read-only).
    /// </summary>
    public float CurrentSpeed => rb.linearVelocity.magnitude;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;          // Top-down / side-view without gravity
        rb.linearDamping = linearDrag; // Use linearDamping instead of deprecated drag
        rb.freezeRotation = true;      // We handle rotation manually if needed
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    /// <summary>
    /// Called by PlayerController each frame to feed input values.
    /// </summary>
    public void SetInput(float horizontal, float vertical)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
    }

    /// <summary>
    /// Applies forces to the Rigidbody2D based on current input.
    /// </summary>
    void ApplyMovement()
    {
        Vector2 desiredVelocity = Vector2.zero;

        // Horizontal movement
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            desiredVelocity.x = horizontalInput * maxSpeed;
        }

        // Accelerate / Brake logic
        if (verticalInput > 0.01f)
        {
            // Accelerating – increase max speed temporarily (boost feel)
            desiredVelocity.x = horizontalInput * (maxSpeed * 1.2f);
        }
        else if (verticalInput < -0.01f)
        {
            // Braking – apply strong deceleration
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, brakeForce * Time.fixedDeltaTime);
            return; // Skip normal force application when braking
        }

        // Smoothly interpolate current velocity toward desired velocity
        Vector2 newVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );

        // Apply deceleration when no horizontal input
        if (Mathf.Abs(horizontalInput) < 0.01f && verticalInput > -0.01f)
        {
            newVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = newVelocity;
    }

    /// <summary>
    /// Instantly stops the bike (e.g., on game over or restart).
    /// </summary>
    public void StopBike()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Resets bike physics state.
    /// </summary>
    public void ResetBike()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = Vector3.zero;
    }
}

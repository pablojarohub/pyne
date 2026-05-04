using UnityEngine;

/// <summary>
/// PlayerController handles player input and delegates movement to BikeController.
/// Also manages delivery state and wobble animations.
/// </summary>
[RequireComponent(typeof(BikeController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Input")]
    [Tooltip("Horizontal move speed multiplier")]
    public float moveSpeed = 8f;

    [Header("Wobble Settings")]
    [Tooltip("How much the bike wobbles while moving")]
    public float wobbleAmount = 5f;
    [Tooltip("Speed of the wobble animation")]
    public float wobbleSpeed = 10f;

    [Header("Visuals")]
    public Transform bikeVisuals;      // Assign the child sprite/transform here
    public Transform characterVisuals; // Assign the character sprite here

    // Internal
    private BikeController bike;
    private float currentWobble;
    private float wobbleTimer;
    private Vector3 originalBikeRotation;
    private Vector3 originalCharacterRotation;

    // Delivery state
    public bool IsCarryingOrder { get; private set; }

    void Awake()
    {
        bike = GetComponent<BikeController>();
        if (bikeVisuals != null)
            originalBikeRotation = bikeVisuals.localEulerAngles;
        if (characterVisuals != null)
            originalCharacterRotation = characterVisuals.localEulerAngles;
    }

    void Update()
    {
        HandleInput();
        ApplyWobble();
    }

    /// <summary>
    /// Reads player input and passes movement commands to BikeController.
    /// </summary>
    void HandleInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f; // accelerate
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f; // brake

        bike.SetInput(horizontal, vertical);

        // Flip visuals based on direction
        if (horizontal != 0 && bikeVisuals != null)
        {
            Vector3 scale = bikeVisuals.localScale;
            scale.x = Mathf.Sign(horizontal) * Mathf.Abs(scale.x);
            bikeVisuals.localScale = scale;
        }
    }

    /// <summary>
    /// Adds a humorous wobble effect proportional to current speed.
    /// </summary>
    void ApplyWobble()
    {
        if (bikeVisuals == null) return;

        float speedRatio = bike.CurrentSpeed / bike.maxSpeed;
        if (speedRatio > 0.05f)
        {
            wobbleTimer += Time.deltaTime * wobbleSpeed * (1f + speedRatio);
            currentWobble = Mathf.Sin(wobbleTimer) * wobbleAmount * speedRatio;
        }
        else
        {
            // Return to neutral when stopped
            currentWobble = Mathf.Lerp(currentWobble, 0f, Time.deltaTime * 5f);
        }

        bikeVisuals.localRotation = Quaternion.Euler(0f, 0f, currentWobble);

        if (characterVisuals != null)
        {
            // Character wobbles slightly out of phase for comedic effect
            float charWobble = Mathf.Sin(wobbleTimer * 1.3f) * (wobbleAmount * 0.5f * speedRatio);
            characterVisuals.localRotation = Quaternion.Euler(0f, 0f, charWobble);
        }
    }

    /// <summary>
    /// Call this when the player picks up an order.
    /// </summary>
    public void PickupOrder()
    {
        if (IsCarryingOrder) return;
        IsCarryingOrder = true;
        // TODO: Trigger pickup animation / sound
    }

    /// <summary>
    /// Call this when the player successfully delivers an order.
    /// </summary>
    public void DeliverOrder()
    {
        if (!IsCarryingOrder) return;
        IsCarryingOrder = false;
        // TODO: Trigger delivery animation / sound
    }

    /// <summary>
    /// Resets player state (used on game restart).
    /// </summary>
    public void ResetPlayer()
    {
        IsCarryingOrder = false;
        bike.ResetBike();
        wobbleTimer = 0f;
        currentWobble = 0f;
        if (bikeVisuals != null)
            bikeVisuals.localRotation = Quaternion.Euler(originalBikeRotation);
        if (characterVisuals != null)
            characterVisuals.localRotation = Quaternion.Euler(originalCharacterRotation);
    }
}

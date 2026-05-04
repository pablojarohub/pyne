using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DeliveryManager handles the core delivery loop:
/// - Spawning pickup points (restaurants)
/// - Assigning random delivery destinations
/// - Tracking order state and scoring
/// </summary>
public class DeliveryManager : MonoBehaviour
{
    [Header("Locations")]
    [Tooltip("All possible pickup locations (restaurants/shops)")]
    public List<Transform> pickupPoints = new List<Transform>();
    [Tooltip("All possible delivery destinations (houses/buildings)")]
    public List<Transform> deliveryPoints = new List<Transform>();

    [Header("UI References")]
    public UIManager uiManager;

    [Header("Scoring")]
    [Tooltip("Base points for completing a delivery")]
    public int baseDeliveryScore = 100;
    [Tooltip("Time threshold (seconds) for fast-delivery bonus")]
    public float fastDeliveryThreshold = 15f;
    [Tooltip("Bonus multiplier for fast deliveries")]
    public float fastDeliveryMultiplier = 1.5f;

    [Header("Difficulty Scaling")]
    [Tooltip("Reduce allowed time by this amount per completed delivery")]
    public float timeReductionPerDelivery = 1f;
    [Tooltip("Minimum allowed delivery time")]
    public float minDeliveryTime = 8f;

    // Events
    public System.Action OnOrderPickedUp;
    public System.Action<int> OnOrderDelivered; // int = score earned
    public System.Action OnGameOver;

    // State
    public enum DeliveryState { Idle, GoingToPickup, CarryingOrder, Delivered }
    public DeliveryState CurrentState { get; private set; } = DeliveryState.Idle;

    private Transform currentPickup;
    private Transform currentDelivery;
    private float orderStartTime;
    private float currentTimeLimit;
    private int totalDeliveries;
    private int currentScore;

    void Start()
    {
        currentTimeLimit = fastDeliveryThreshold;
        StartNewDelivery();
    }

    void Update()
    {
        if (CurrentState == DeliveryState.CarryingOrder)
        {
            float elapsed = Time.time - orderStartTime;
            float remaining = Mathf.Max(0f, currentTimeLimit - elapsed);
            uiManager?.UpdateTimer(remaining, currentTimeLimit);

            if (remaining <= 0f)
            {
                TriggerGameOver("Time's up! The food got cold.");
            }
        }
    }

    /// <summary>
    /// Starts a new delivery cycle by selecting a random pickup point.
    /// </summary>
    public void StartNewDelivery()
    {
        if (pickupPoints.Count == 0 || deliveryPoints.Count == 0)
        {
            Debug.LogError("[DeliveryManager] No pickup or delivery points assigned!");
            return;
        }

        CurrentState = DeliveryState.GoingToPickup;
        currentPickup = GetRandomPoint(pickupPoints);
        currentDelivery = null;

        uiManager?.ShowTask("Go to " + GetLocationName(currentPickup) + " to pick up the order!");
        uiManager?.ShowArrow(currentPickup.position);
    }

    /// <summary>
    /// Call this when the player reaches the pickup point.
    /// </summary>
    public void PickupOrder()
    {
        if (CurrentState != DeliveryState.GoingToPickup) return;

        CurrentState = DeliveryState.CarryingOrder;
        orderStartTime = Time.time;

        // Select a delivery point different from pickup
        do
        {
            currentDelivery = GetRandomPoint(deliveryPoints);
        } while (currentDelivery == currentPickup);

        uiManager?.ShowTask("Deliver to " + GetLocationName(currentDelivery) + "!");
        uiManager?.ShowArrow(currentDelivery.position);
        OnOrderPickedUp?.Invoke();
    }

    /// <summary>
    /// Call this when the player reaches the delivery point.
    /// </summary>
    public void DeliverOrder()
    {
        if (CurrentState != DeliveryState.CarryingOrder) return;

        float elapsed = Time.time - orderStartTime;
        bool isFast = elapsed <= currentTimeLimit;
        int score = CalculateScore(isFast);
        currentScore += score;
        totalDeliveries++;

        // Increase difficulty
        currentTimeLimit = Mathf.Max(minDeliveryTime, fastDeliveryThreshold - (totalDeliveries * timeReductionPerDelivery));

        CurrentState = DeliveryState.Delivered;
        uiManager?.UpdateScore(currentScore);
        uiManager?.ShowTask("Delivery complete! +" + score + " points");
        OnOrderDelivered?.Invoke(score);

        // Start next delivery after a short delay
        Invoke(nameof(StartNewDelivery), 1.5f);
    }

    /// <summary>
    /// Returns the current target transform based on state.
    /// </summary>
    public Transform GetCurrentTarget()
    {
        return CurrentState == DeliveryState.GoingToPickup ? currentPickup : currentDelivery;
    }

    /// <summary>
    /// Returns the player's current score.
    /// </summary>
    public int GetScore() => currentScore;

    /// <summary>
    /// Returns total completed deliveries.
    /// </summary>
    public int GetTotalDeliveries() => totalDeliveries;

    int CalculateScore(bool isFast)
    {
        int score = baseDeliveryScore;
        if (isFast)
            score = Mathf.RoundToInt(score * fastDeliveryMultiplier);
        return score;
    }

    Transform GetRandomPoint(List<Transform> points)
    {
        return points[Random.Range(0, points.Count)];
    }

    string GetLocationName(Transform point)
    {
        if (point == null) return "Unknown";
        // Use GameObject name or a custom name component
        LocationName locName = point.GetComponent<LocationName>();
        return locName != null ? locName.displayName : point.name;
    }

    void TriggerGameOver(string reason)
    {
        CurrentState = DeliveryState.Idle;
        uiManager?.ShowGameOver(currentScore, totalDeliveries, reason);
        OnGameOver?.Invoke();
    }

    /// <summary>
    /// Resets the delivery system for a new game.
    /// </summary>
    public void ResetDeliveries()
    {
        CurrentState = DeliveryState.Idle;
        currentScore = 0;
        totalDeliveries = 0;
        currentTimeLimit = fastDeliveryThreshold;
        currentPickup = null;
        currentDelivery = null;
        CancelInvoke();
        StartNewDelivery();
    }
}

/// <summary>
/// Simple component to assign a display name to a location GameObject.
/// </summary>
public class LocationName : MonoBehaviour
{
    public string displayName = "Location";
}

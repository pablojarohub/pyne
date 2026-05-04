using UnityEngine;

/// <summary>
/// Optional combo system that rewards rapid consecutive deliveries.
/// Attach to the DeliveryManager or GameManager.
/// </summary>
public class ComboSystem : MonoBehaviour
{
    [Tooltip("Time window to chain deliveries for combo")]
    public float comboWindow = 10f;
    [Tooltip("Score multiplier per combo level")]
    public float comboMultiplier = 0.5f;
    [Tooltip("Max combo multiplier")]
    public float maxMultiplier = 3f;

    public int CurrentCombo { get; private set; }
    public float CurrentMultiplier => 1f + Mathf.Min(CurrentCombo * comboMultiplier, maxMultiplier - 1f);

    private float lastDeliveryTime = -999f;

    void OnEnable()
    {
        var dm = GetComponent<DeliveryManager>();
        if (dm != null)
            dm.OnOrderDelivered += OnDelivery;
    }

    void OnDisable()
    {
        var dm = GetComponent<DeliveryManager>();
        if (dm != null)
            dm.OnOrderDelivered -= OnDelivery;
    }

    void OnDelivery(int baseScore)
    {
        float timeSinceLast = Time.time - lastDeliveryTime;
        if (timeSinceLast <= comboWindow)
        {
            CurrentCombo++;
            Debug.Log($"Combo x{CurrentCombo}! Multiplier: {CurrentMultiplier:F1}x");
        }
        else
        {
            CurrentCombo = 1;
        }
        lastDeliveryTime = Time.time;
    }

    public void ResetCombo()
    {
        CurrentCombo = 0;
        lastDeliveryTime = -999f;
    }
}

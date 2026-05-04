using UnityEngine;

/// <summary>
/// Attach this to pickup and delivery point GameObjects.
/// Handles trigger detection when the player enters the zone.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DeliveryPoint : MonoBehaviour
{
    [Tooltip("Is this a restaurant/shop (pickup) or a house (delivery)?")]
    public bool isPickupPoint = true;

    [Tooltip("Visual indicator when active")]
    public GameObject activeIndicator;

    [Tooltip("Particle effect on interaction")]
    public ParticleSystem interactionParticles;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        DeliveryManager dm = GameManager.Instance?.deliveryManager;
        if (dm == null) return;

        if (isPickupPoint && dm.CurrentState == DeliveryManager.DeliveryState.GoingToPickup)
        {
            // Check if this is the currently assigned pickup
            if (dm.GetCurrentTarget() == transform)
            {
                dm.PickupOrder();
                PlayEffects();
            }
        }
        else if (!isPickupPoint && dm.CurrentState == DeliveryManager.DeliveryState.CarryingOrder)
        {
            // Check if this is the currently assigned delivery
            if (dm.GetCurrentTarget() == transform)
            {
                dm.DeliverOrder();
                PlayEffects();
            }
        }
    }

    void PlayEffects()
    {
        if (interactionParticles != null)
            interactionParticles.Play();
    }

    /// <summary>
    /// Call this to show/hide the active indicator.
    /// </summary>
    public void SetActiveVisual(bool active)
    {
        if (activeIndicator != null)
            activeIndicator.SetActive(active);
    }
}

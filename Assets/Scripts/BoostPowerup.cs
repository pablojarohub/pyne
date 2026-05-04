using UnityEngine;

/// <summary>
/// Temporary speed boost pickup. Optional feature for extra arcade fun.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BoostPowerup : MonoBehaviour
{
    [Tooltip("Speed multiplier while boosted")]
    public float boostMultiplier = 1.8f;
    [Tooltip("How long the boost lasts")]
    public float boostDuration = 3f;
    [Tooltip("Rotation speed for visual effect")]
    public float spinSpeed = 180f;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public ParticleSystem collectParticles;

    [Header("Audio")]
    public AudioClip collectSound;

    void Update()
    {
        // Spin the powerup for visibility
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        BikeController bike = other.GetComponent<BikeController>();
        if (bike != null)
        {
            bike.StartCoroutine(ApplyBoost(bike));
        }

        GameManager.Instance?.PlaySFX(collectSound);
        if (collectParticles != null)
            Instantiate(collectParticles, transform.position, Quaternion.identity);

        // Hide instead of destroy so it can respawn
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), 10f);
    }

    System.Collections.IEnumerator ApplyBoost(BikeController bike)
    {
        float originalMaxSpeed = bike.maxSpeed;
        bike.maxSpeed *= boostMultiplier;

        // Visual feedback on player could go here
        yield return new WaitForSeconds(boostDuration);

        bike.maxSpeed = originalMaxSpeed;
    }

    void Respawn()
    {
        gameObject.SetActive(true);
    }
}

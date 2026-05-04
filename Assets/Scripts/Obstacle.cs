using UnityEngine;

/// <summary>
/// Simple obstacle that pushes the player back on collision.
/// Optional: damages/score penalty can be added here.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [Tooltip("Force applied to player on collision")]
    public float bounceForce = 10f;

    [Tooltip("Sound effect on hit")]
    public AudioClip hitSound;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 bounceDir = (collision.transform.position - transform.position).normalized;
            playerRb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
        }

        GameManager.Instance?.PlaySFX(hitSound);
    }
}

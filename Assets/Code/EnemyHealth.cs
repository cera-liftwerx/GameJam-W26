using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public int health = 100;

    // This runs when the bubble "dies" against the enemy's collider
    private void OnParticleCollision(GameObject other) {
        // Double check the tag to be safe
        TakeDamage(10);
        Debug.Log("Enemy gassed! Remaining health: " + health);
    }

    public void TakeDamage(int amount) {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die() {
        // Optional: Trigger a green explosion here later
        Destroy(gameObject);
    }
}
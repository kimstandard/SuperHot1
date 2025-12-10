using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth;

    public GameManager gameManager;
    public RagdollController ragdollController;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //ragdollController.EnableRagdoll();
        gameManager.OnPlayerDied();
        Debug.Log("die");
    }
}

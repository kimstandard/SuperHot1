using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        Vector3 pos = collision.contacts[0].point;

        RagdollChanger ragdollChanger = collision.collider.GetComponentInParent<RagdollChanger>();
        if (ragdollChanger != null)
        {
            ragdollChanger.TakeDamage(pos);
        }

        Destroy(gameObject);
    }
}
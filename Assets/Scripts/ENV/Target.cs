using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float health = 50f;
    public GameObject DestroyedModel;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(DestroyedModel != null)
        {
            Instantiate(DestroyedModel, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

}

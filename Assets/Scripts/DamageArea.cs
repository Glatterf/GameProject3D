using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damagePerSecond = 10f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
using UnityEngine;

public class Obtainable : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObtainablesCounter.Instance.AddObtainable();
            Destroy(gameObject);
        }
    }
}
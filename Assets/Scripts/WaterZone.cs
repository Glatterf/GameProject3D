using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void Start()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }
}
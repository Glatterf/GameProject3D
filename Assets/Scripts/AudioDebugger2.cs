using UnityEngine;

public class AudioDebugger2 : MonoBehaviour
{
    void Update()
    {
        // Debug: Check if player is moving
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            Debug.Log("📍 Player moving...");
        }

        // Test footstep manually
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("🔊 Testing Soil Footstep...");
            AudioManager.Instance.PlayFootstep("soil");
            
            Debug.Log("🔊 Testing Rock Footstep...");
            AudioManager.Instance.PlayFootstep("rock");
            
            Debug.Log("🔊 Testing Water Footstep...");
            AudioManager.Instance.PlayFootstep("water");
        }

        // Test proximity
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("🔊 Testing Proximity...");
            AudioManager.Instance.PlayProximityNoise(transform.position);
        }
    }
}
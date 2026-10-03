using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== AUDIO DEBUG ===");

        // Check 1: AudioManager exists
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL! Create AudioManager GameObject!");
            return;
        }
        Debug.Log("✅ AudioManager found");

        // Check 2: Player tagged correctly
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No GameObject with 'Player' tag found!");
        }
        else
        {
            Debug.Log("✅ Player found and tagged");
        }

        // Check 3: Start BGM
        Debug.Log("🎵 Starting BGM...");
        AudioManager.Instance.PlayBGM();
        Debug.Log("✅ PlayBGM() called");
    }

    void Update()
    {
        // Check if audio is actually playing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("🔊 Testing footstep sound...");
            AudioManager.Instance.PlayFootstep("soil");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("🔊 Testing proximity sound...");
            AudioManager.Instance.PlayProximityNoise(transform.position);
        }
    }
}
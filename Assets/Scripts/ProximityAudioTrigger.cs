using UnityEngine;

public class ProximityAudioTrigger : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField, Range(0f, 1f)] private float volume = 0.4f;
    [Tooltip("1 = fully 3D (fades with distance), 0 = flat 2D")]
    [SerializeField, Range(0f, 1f)] private float spatialBlend = 0.7f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private AudioSource source;
    private Transform player;
    private int lastIndex = -1;
    private bool wasInRange = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError($"[{name}] No GameObject tagged 'Player' found!", this);
            enabled = false;
            return;
        }
        player = playerObj.transform;

        if (clips == null || clips.Length == 0)
            Debug.LogWarning($"[{name}] Clips array is EMPTY. Assign audio in this sphere's Inspector.", this);

        if (FindFirstObjectByType<AudioListener>() == null)
            Debug.LogError("No AudioListener in the scene! Add one to the Main Camera.", this);

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = detectionRadius * 0.3f;
        source.maxDistance = detectionRadius;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance < detectionRadius;

        if (debugMode && inRange != wasInRange)
            Debug.Log($"[{name}] Player {(inRange ? "ENTERED" : "LEFT")} range (distance {distance:F1})", this);
        wasInRange = inRange;

        if (inRange)
        {
            if (!source.isPlaying) PlayNextClip();
        }
        else if (source.isPlaying)
        {
            source.Stop();
        }
    }

    private void PlayNextClip()
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1)
            while (index == lastIndex) index = Random.Range(0, clips.Length);

        lastIndex = index;
        source.clip = clips[index];
        source.Play();

        if (debugMode) Debug.Log($"[{name}] Playing: {clips[index].name}", this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
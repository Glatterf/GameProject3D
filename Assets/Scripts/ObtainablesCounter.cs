using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ObtainablesCounter : MonoBehaviour
{
    public static ObtainablesCounter Instance;
    public TextMeshProUGUI counterText;

    public UnityEvent OnAllCollected;

    private int count = 0;
    private int totalObtainables = 0;

 void Awake()
{
    Instance = this;
    Debug.Log("ObtainablesCounter Awake on: " + gameObject.name);
}

    void Start()
    {
        // Auto-count every Obtainable already placed in the scene
        totalObtainables = FindObjectsOfType<Obtainable>().Length;
        UpdateText();
    }

 public void AddObtainable()
{
    count++;
    UpdateText();
    Debug.Log("Count: " + count + " / " + totalObtainables);

    if (count >= totalObtainables)
    {
        Debug.Log("All collected - invoking event");
        OnAllCollected?.Invoke();
    }
}

    void UpdateText()
    {
        counterText.text = "Obtainables: " + count + " / " + totalObtainables;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public Slider healthBar;

    public void UpdateHealthBar(float current, float max)
    {
        healthBar.maxValue = max;
        healthBar.value = current;
    }
}
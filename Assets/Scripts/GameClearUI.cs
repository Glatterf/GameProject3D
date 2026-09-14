using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearUI : MonoBehaviour
{
    public GameObject gameClearPanel;

   public void ShowGameClear()
{
    Debug.Log("ShowGameClear called");
    gameClearPanel.SetActive(true);
    Time.timeScale = 0f;

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonControl : MonoBehaviour
{
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ClearItem()
    {
        Player.Instance.GetItemObject().DestroySelf();
    }
}

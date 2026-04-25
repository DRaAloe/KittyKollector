using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Play clicked");
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.T))
    {
        ClaudeManager.Instance.Ask("Say hello to the player in 1 sentence.", reply =>
        {
            Debug.Log("Claude says: " + reply);
        });
    }
}
}
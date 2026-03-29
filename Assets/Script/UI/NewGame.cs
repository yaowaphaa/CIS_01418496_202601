using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void StartNewGame()
    {
        GameProgress.ResetProgress();
        BossHealth.savedHealth = -1f;
        BossHealth.savedHealthCheckpoint = -1f;
        SceneManager.LoadScene("SelectMap1");
    }
}
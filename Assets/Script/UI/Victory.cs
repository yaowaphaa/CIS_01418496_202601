using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainM : MonoBehaviour
{

    public void Main()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void PlayAg()
    {
        GameProgress.ResetProgress();
        BossHealth.savedHealth = -1f;
        BossHealth.savedHealthCheckpoint = -1f;
        SceneManager.LoadScene("ProjectLevel1");
    }
    
}
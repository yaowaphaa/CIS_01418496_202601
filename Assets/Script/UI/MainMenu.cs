using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject continueBtn;  // ปุ่มไปต่อ
    public GameObject newGameBtn;   // ปุ่มเริ่มใหม่

    void Start()
    {
        bool hasData = PlayerPrefs.GetInt("highestStageCleared", 0) > 0;
        
        continueBtn.GetComponent<Button>().interactable = hasData;
    }

    public void Continue()
    {
        SceneManager.LoadScene("SelectMap1");
    }

    public void StartNewGame()
    {
        GameProgress.ResetProgress();
        BossHealth.savedHealth = -1f;
        BossHealth.savedHealthCheckpoint = -1f;
        SceneManager.LoadScene("SelectMap1");
    }
    
}
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SelectMap1 : MonoBehaviour
{
    public string gameSceneName = "ProjectLevel1"; 

    public void StartGame()
    {
        
        SceneManager.LoadScene(gameSceneName);
    }
}
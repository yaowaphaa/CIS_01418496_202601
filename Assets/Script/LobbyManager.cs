using UnityEngine;
using UnityEngine.SceneManagement; 

public class LobbyManager : MonoBehaviour
{
    public string gameSceneName = "SelectMap1"; 

    public void LoadLevel1()
    {
        
        SceneManager.LoadScene(gameSceneName);
    }

    /*public void ExitGame()
    {
        // สั่งปิดโปรแกรม
        Debug.Log("Game is Exiting...");
        Application.Quit();
    }*/
}
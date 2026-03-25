using UnityEngine;
using UnityEngine.SceneManagement; 

public class LobbyManager : MonoBehaviour
{ 
    public void LoadLevel1()
    {
        
        SceneManager.LoadScene("SelectMap1");
    }

    /*public void ExitGame()
    {
        // สั่งปิดโปรแกรม
        Debug.Log("Game is Exiting...");
        Application.Quit();
    }*/
}
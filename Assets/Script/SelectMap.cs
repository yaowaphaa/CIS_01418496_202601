using UnityEngine;
using UnityEngine.SceneManagement; 

public class SelectMap1 : MonoBehaviour
{
    public void StartGame()
    {
        
        SceneManager.LoadScene("ProjectLevel1");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public string targetScene = "SelectMap1"; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
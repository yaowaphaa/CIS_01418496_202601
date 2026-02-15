using UnityEngine;

public class Score : MonoBehaviour
{
    public static int score = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Debug.Log(Score.score += 1); // แสดงผลในคอนโซล
            Destroy(gameObject);       // ทำให้ของหาย
        }
    }
}

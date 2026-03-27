using UnityEngine;

public class Score : MonoBehaviour
{
    public static int score = 0;   // เหรียญสะสมทั้งหมด

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            score += 1; // 💰 เพิ่มเหรียญสะสม
            

            // 🔵 เพิ่ม Mana ในด่าน
            PlayerAttack player = other.GetComponent<PlayerAttack>();
            if (player != null)
            {
                player.AddMana(1);
            }

            Destroy(gameObject);
        }
    }
}
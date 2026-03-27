using UnityEngine;

public class Score : MonoBehaviour
{
    public static int score = 0;   // เหรียญสะสมทั้งหมดในเกม

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            score += 1; // 💰 เพิ่มเหรียญสะสมเกม

            // เพิ่ม Mana และ totalCoins ใน PlayerAttack
            PlayerAttack player = other.GetComponent<PlayerAttack>();
            if (player != null)
            {
                player.AddMana(1); // เพิ่มทั้ง battleMana และ totalCoins
            }

            Destroy(gameObject);
        }
    }
}
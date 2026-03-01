using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int damage;
    public float cooldown;
    public int manaCost;

    [HideInInspector]
    public float lastUsedTime;
    public GameObject projectilePrefab;

    public bool CanUse()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void Use()
    {
        lastUsedTime = Time.time;
    }
}
//ลาก  prefeb Fireball ไปใส่ใน Projectile Prefab
// game objact ชื่อ FirePoint ใส่ FirePoint ใน inspactor
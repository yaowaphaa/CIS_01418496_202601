using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress instance;
    public static int highestStageCleared => PlayerPrefs.GetInt("highestStageCleared", 0);

    public static string[] minionScenes = new string[]
    { "ProjectLevel1", "ProjectLevel2", "ProjectLevel3", "ProjectLevel4" };

    public static string[] bossScenes = new string[]
    { "BossScene", "BossScene1", "BossScene2", "BossScene3" };

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool IsMinionUnlocked(int minionIndex)
    {
        return minionIndex <= highestStageCleared;
    }

    public static void ClearBoss(int bossIndex)
    {
        int current = PlayerPrefs.GetInt("highestStageCleared", 0);
        if (bossIndex + 1 > current)
        {
            PlayerPrefs.SetInt("highestStageCleared", bossIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("highestStageCleared", 0);
        PlayerPrefs.Save();
    }
}
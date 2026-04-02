using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI stageLabel;
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI badgeText;
    public Image badgeBG;
    public Button prevBtn;
    public Button nextBtn;
    public Button playBtn;
    public Image[] dots;

    [Header("Colors")]
    public Color colorDone = new Color(0.13f, 0.6f, 0.36f);
    public Color colorOpen = new Color(0.1f, 0.4f, 0.8f);
    public Color colorLock = new Color(0.5f, 0.5f, 0.5f);
    public Color dotActive = new Color(0.1f, 0.1f, 0.1f);
    public Color dotInactive = new Color(0.7f, 0.7f, 0.7f);

    private string[] names = { "1", "2", "3", "4" };
    private int cur = 0;

    void Start()
    {
        prevBtn.onClick.AddListener(() => Move(-1));
        nextBtn.onClick.AddListener(() => Move(1));
        playBtn.onClick.AddListener(Play);
        Render();
    }

    void Move(int dir)
    {
        cur = Mathf.Clamp(cur + dir, 0, 3);
        Render();
    }

    void Render()
    {
        stageLabel.text = "LEVEL" + (cur + 1);
        stageName.text = names[cur];

        bool unlocked = GameProgress.IsMinionUnlocked(cur);
        bool cleared = cur < GameProgress.highestStageCleared;

        if (!unlocked)
        {
            badgeText.text = "LOKED";
            badgeText.color = Color.white;
            badgeBG.color = colorLock;
        }
        else if (cleared)
        {
            badgeText.text = "PASSED";
            badgeText.color = Color.white;
            badgeBG.color = colorDone;
        }
        else
        {
            badgeText.text = "UNLOCKED";
            badgeText.color = Color.white;
            badgeBG.color = colorOpen;
        }

        playBtn.interactable = unlocked;
        prevBtn.interactable = cur > 0;
        nextBtn.interactable = cur < 3;

        for (int i = 0; i < dots.Length; i++)
            dots[i].color = i == cur ? dotActive : dotInactive;
    }

    void Play()
    {
        if (cur == 0)
        {
            // เริ่มใหม่
            BossHealth.savedHealth = -1f;
            BossHealth.savedHealthCheckpoint = -1f;

            PlayerAttack.savedMana = 0;
            PlayerAttack.savedCoins = 0;
        }
        else
        {
            float hp = PlayerPrefs.GetFloat("bossHealth", -1f);
            PlayerPrefs.SetFloat("bossHealthStageStart", hp);
        }

        SceneManager.LoadScene(GameProgress.minionScenes[cur]);
    }
}
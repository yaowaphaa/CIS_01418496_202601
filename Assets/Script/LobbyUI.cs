using UnityEngine;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    void Start()
    {
        if (coinText != null)
            coinText.text = PlayerAttack.savedCoins.ToString();
    }
}
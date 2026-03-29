using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class BossSceneController : MonoBehaviour
{
    public CinemachineCamera playerCam; // กล้องที่ตัวละคร
    public CinemachineCamera bossCam;   // กล้องที่บอส
    public PlayerBossMovement player;   // ลากตัว Player มาใส่ช่องนี้
    public GameObject gameUI;

    IEnumerator Start()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        playerCam.Priority = 20;
        bossCam.Priority = 10;
        if (player.childAnim != null) player.childAnim.SetBool("IsFallingIdle", true);
        yield return new WaitUntil(() => player.isLanded);
        yield return new WaitForSeconds(0.8f);
        playerCam.Priority = 10;
        bossCam.Priority = 20;
        yield return new WaitForSeconds(3.0f); 
        playerCam.Priority = 20;
        bossCam.Priority = 10;
        yield return new WaitForSeconds(1.5f); 
        if (gameUI != null) 
        {
            gameUI.SetActive(true);
        }
        player.isIntroPlaying = false;
    }
}
using UnityEngine;
using System.Collections;

public class SmartPortal : MonoBehaviour
{
    private Vector3 fullScale; 
    private bool isRunning = false;

    void Awake()
    {
        fullScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void ActivatePortal(Vector3 pos)
    {
        if (isRunning) return; 

        transform.position = pos;
        gameObject.SetActive(true);
        
        StartCoroutine(ScaleAndMoveRoutine());
    }

    IEnumerator ScaleAndMoveRoutine()
    {
        isRunning = true;

        // 1. จำตำแหน่ง "เป๊ะๆ" ที่บอสส่งมา (ไม่ว่าจะเป็น 1 หรือ 60)
        Vector3 fixedPosition = transform.position;
        
        float time = 0;
        float duration = 3f; 

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = fixedPosition;
            transform.localScale = Vector3.MoveTowards(transform.localScale, fullScale, 2f * Time.deltaTime);
            yield return null;
        }

        // ปิดงานให้เป๊ะ
        transform.position = fixedPosition;
        isRunning = false;
    }
}
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    
    public Transform player; 
    public float forwardDistance = 12f; 
    public float smoothTime = 0.2f;     
    public float activationRange = 15f;
    private Vector3 currentVelocity = Vector3.zero;
    private bool hasSeenPlayer = false; 
    private bool isFrozen = false;

    public void FreezeMovement(bool freeze)
    {
        isFrozen = freeze;
    }

    void LateUpdate()
    {
        if (player == null) return;
        if (isFrozen) return;
        float distanceToPlayer = transform.position.x - player.position.x;
        if (!hasSeenPlayer && distanceToPlayer <= activationRange)
        {
            hasSeenPlayer = true;
            Debug.Log("Boss Activated!");
        }
        if (hasSeenPlayer)
        {
            Vector3 targetPosition = new Vector3(player.position.x + forwardDistance, transform.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref currentVelocity, 
                smoothTime
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
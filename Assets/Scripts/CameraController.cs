using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector2 followOffset;
    public Vector2 noFollowZoneSize;
    public Vector2 minCameraPosition;
    public Vector2 maxCameraPosition;

    private Vector2 noFollowZoneCenter;
    private bool followPlayer = false;

    void Start()
    {
        noFollowZoneCenter = transform.position;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (followPlayer)
        {
            // Maintenir la position Z de la caméra
            Vector3 playerPositionWithOffset = new Vector3(player.position.x + followOffset.x, player.position.y + followOffset.y, transform.position.z);
            targetPosition = Vector3.Lerp(transform.position, playerPositionWithOffset, followSpeed * Time.deltaTime);
        }
        else
        {
            if (Mathf.Abs(player.position.x - transform.position.x) > noFollowZoneSize.x || Mathf.Abs(player.position.y - transform.position.y) > noFollowZoneSize.y)
            {
                followPlayer = true;
            }
        }

        // Limites de déplacement de la caméra
        targetPosition.x = Mathf.Clamp(targetPosition.x, minCameraPosition.x, maxCameraPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraPosition.y, maxCameraPosition.y);

        transform.position = targetPosition;
    }
}

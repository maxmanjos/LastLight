using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         // Player transform
    public Vector3 offset = new Vector3(0, 2, -4);  // Position behind player
    public float followSpeed = 10f;  // Smooth following

    void LateUpdate()
    {
        if (player == null) return;

        // Rotate the offset to match the player's facing direction
        Vector3 rotatedOffset = player.rotation * offset;

        // Set camera position behind the player
        Vector3 targetPosition = player.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Make the camera look where the player is looking
        transform.rotation = Quaternion.Lerp(
            transform.rotation, 
            Quaternion.Euler(0, player.eulerAngles.y, 0), 
            followSpeed * Time.deltaTime
        );
    }
}

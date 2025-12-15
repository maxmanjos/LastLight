using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 150f;
    public Transform playerBody;    // drag your Player here in Inspector

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // hides + locks cursor
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // prevents flipping
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal look (rotate player)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;
    private float xRotation = 0f;

    void Update()
    {
        float sens = PlayerPrefs.GetFloat("sensitivity", 5f);

        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
using UnityEngine;

public class CameraFly : MonoBehaviour
{
    public float speed = 2f;
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S
        float y = 0;

        if (Input.GetKey(KeyCode.E)) y = 1;   // E = up
        if (Input.GetKey(KeyCode.Q)) y = -1;  // Q = down

        Vector3 move = transform.forward * v + transform.right * h + Vector3.up * y;
        transform.position += move * speed * Time.deltaTime;

        // Optional: rotate with mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(-mouseY, mouseX, 0);
    }
}


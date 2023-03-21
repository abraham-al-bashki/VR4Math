using UnityEngine;

public class SceneViewCameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 50f;
    public float verticalMovementSpeed = 2f;

    private Vector3 lastMousePosition;
    private bool followMouse = true;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // Check if we should follow the mouse or not
        if (!followMouse)
        {
            if (Input.GetMouseButtonDown(1)) // Right-click to resume following the mouse
            {
                followMouse = true;
            }
            return;
        }

        // Get input from the arrow keys or WASD to move the camera
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upInput = Input.GetAxis("Jump"); // Assumes the space bar is used to move up
        float downInput = Input.GetAxis("Submit"); // Assumes the control key is used to move down

        Vector3 positionDelta = new Vector3(horizontalInput, 0f, verticalInput) * movementSpeed * Time.deltaTime;
        transform.position += transform.TransformDirection(positionDelta);

        // Move the camera up or down based on input
        float verticalDelta = (upInput - downInput) * verticalMovementSpeed * Time.deltaTime;
        transform.position += Vector3.up * verticalDelta;

        // Get input from the mouse to rotate the camera
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        float mouseXInput = mouseDelta.x;
        float mouseYInput = mouseDelta.y;

        Vector3 rotationDelta = new Vector3(-mouseYInput, mouseXInput, 0f) * rotationSpeed * Time.deltaTime;
        transform.eulerAngles += rotationDelta;

        lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(1)) // Right-click to stop following the mouse
        {
            followMouse = false;
        }
    }
}

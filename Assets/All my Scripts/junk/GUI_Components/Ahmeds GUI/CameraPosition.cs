using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraPosition : MonoBehaviour
{
    /*void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, Screen.height - 50, 400, 40));
        GUILayout.BeginHorizontal();
        GUI.color = Color.white;
        GUI.backgroundColor = Color.black;

        

        // Get the position of the main camera and format it as a string
        Vector3 cameraPosition = Camera.main.transform.position;
        string positionString = string.Format("Camera Position: ({0:F2}, {1:F2}, {2:F2})", cameraPosition.x, cameraPosition.y, cameraPosition.z);

        // Display the camera position in the bottom-left corner of the screen
        GUI.Label(new Rect(10, Screen.height - 50, 400, 40), positionString);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }*/
    void OnGUI()
    {
        // Get a reference to the main camera
        Camera mainCamera = Camera.main;

        // If the main camera is not null, display its position
        if (mainCamera != null)
        {
            Vector3 position = mainCamera.transform.position;

            GUI.color = Color.white;
            GUI.skin.label.fontSize = 25;
            GUI.skin.label.fontStyle = FontStyle.Bold;

            // Define the rect for the box
            Rect boxRect = new Rect(10, Screen.height - 50, 250, 30);

            // Display the box with the position of the main camera
            GUI.Box(boxRect, "Camera Position: " + position.ToString());
        }
    }
}

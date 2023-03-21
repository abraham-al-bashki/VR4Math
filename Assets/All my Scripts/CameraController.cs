using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 0.1f;

    void Update()
    {
        float translationX = Input.GetAxis("Horizontal") * speed;
        float translationZ = Input.GetAxis("Vertical") * speed;
        float translationY = 0.0f;

        if (Input.GetKey(KeyCode.Q))
        {
            translationY -= speed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            translationY += speed;
        }

        transform.Translate(translationX * Time.deltaTime, translationY * Time.deltaTime, translationZ * Time.deltaTime);

        float rotationY = 0.0f;
        float rotationZ = 0.0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationY += rotationSpeed;
            //transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, rotationY, 0) * Time.deltaTime;
            transform.Rotate(0, rotationY, 0 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationY -= rotationSpeed;
            //transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, rotationY, 0) * Time.deltaTime;
            transform.Rotate(0, rotationY, 0 * Time.deltaTime);
        }
        /*if (Input.GetKey(KeyCode.UpArrow))
        {
            rotationZ += rotationSpeed;
            transform.Rotate(rotationZ, 0, 0 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotationZ -= rotationSpeed;
            transform.Rotate(rotationZ, 0, 0 * Time.deltaTime);
        }*/


        //transform.rotation *= Quaternion.AngleAxis(rotationY * Time.deltaTime, Vector3.up);



    }
}


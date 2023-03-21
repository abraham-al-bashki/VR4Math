using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMovement : MonoBehaviour
{
    float duration = 0;
    float startTime = 0;
    float speed = 70;

    float duration_2 = 0;
    float startTime_2 = 0;
    float speed_2 = 360;

    float duration_3 = 0;
    float startTime_3 = 0;
    float speed_3 = 10;
    Quaternion initialRotation_3;
    void FixedUpdate()
    {
        duration = Time.time - startTime;
        if(duration > 100) { startTime = Time.time; }
        if (Input.GetKey(KeyCode.W)) { transform.position = transform.position + transform.forward * Mathf.Lerp(0, speed, duration / 100); }
        else if (Input.GetKey(KeyCode.S)) { transform.position = transform.position - transform.forward * Mathf.Lerp(0, speed, duration / 100); }
        else { startTime = Time.time; }

        duration_2 = Time.time - startTime_2;
        if (duration_2 > 100) { startTime_2 = Time.time; }
        if (Input.GetKey(KeyCode.D)) { transform.eulerAngles = transform.eulerAngles + transform.up * Mathf.Lerp(0, speed_2, duration_2 / 100); }
        else if (Input.GetKey(KeyCode.A)) { transform.eulerAngles = transform.eulerAngles - transform.up * Mathf.Lerp(0, speed_2, duration_2 / 100); }
        else { startTime_2 = Time.time; }

        /*duration_3 = Time.time - startTime_3;
        if (duration_3 > 100) { startTime_3 = Time.time; }
        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q)) { initialRotation_3 = transform.rotation; }
        if (Input.GetKey(KeyCode.E)) { transform.eulerAngles = transform.eulerAngles + transform.forward * Mathf.Lerp(0, speed_3, duration_3 / 100); }
        else if (Input.GetKey(KeyCode.Q)) { transform.eulerAngles = transform.eulerAngles - transform.forward * Mathf.Lerp(0, speed_3, duration_3 / 100); }
        else { startTime_2 = Time.time; transform.rotation = initialRotation_3; }*/
;    }
}

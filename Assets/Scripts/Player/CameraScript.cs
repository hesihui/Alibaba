using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public bool enabled = false;

    public Transform target; // The object to orbit around
    public float distance = 10.0f; // Distance from the target
    public float orbitSpeed = 10.0f; // Speed of rotation
    public float yMinLimit = -20f; // Minimum y value
    public float yMaxLimit = 80f; // Maximum y value
    public float verticalSpeed = 2.0f; // Speed of vertical rotation

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target && enabled)
        {
            // Orbit horizontally around the target
            x += orbitSpeed * Time.deltaTime;

            // Adjust the vertical angle to stay within limits
            y = Mathf.Sin(Time.time * verticalSpeed) * (yMaxLimit - yMinLimit) / 2 + (yMaxLimit + yMinLimit) / 2;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

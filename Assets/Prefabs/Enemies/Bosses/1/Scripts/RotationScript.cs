using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public Quaternion Rotate;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = transform.rotation * Rotate;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCollector : MonoBehaviour
{
    
    void FixedUpdate()
    {
        if(transform.position.y < -50)
            Destroy(gameObject);
    }
}

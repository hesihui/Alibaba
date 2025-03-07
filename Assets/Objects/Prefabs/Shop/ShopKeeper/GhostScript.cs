using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;
    public float waitTime = 1.0f;

    private Transform targetPoint;

    void Start()
    {
        targetPoint = pointA;
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            yield return MoveToPoint(targetPoint);
            yield return RotateDown();
            yield return new WaitForSeconds(waitTime);
            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }

    IEnumerator MoveToPoint(Transform point)
    {
        while (Vector3.Distance(transform.position, point.position) > 0.1f)
        {
            Vector3 direction = (point.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            transform.position = Vector3.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
            yield return null;
        }
        // Ensure precise position and rotation when reaching the target point
        transform.position = point.position;
        yield return null;
    }

    IEnumerator RotateDown()
    {
        Quaternion downRotation = Quaternion.Euler(0f, 180f, 0f); // Adjust the angle as needed
        while (Quaternion.Angle(transform.rotation, downRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, downRotation, Time.deltaTime * speed);
            yield return null;
        }
        transform.rotation = downRotation; // Ensure precise rotation
    }
}

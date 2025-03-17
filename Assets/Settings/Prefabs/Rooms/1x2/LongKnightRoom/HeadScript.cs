using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadScript : MonoBehaviour
{

    public Quaternion headRotation;
    public Vector3 LookUpDistance;

    private Quaternion OriginalLookRotation;
    private Quaternion targetRotation;
    private bool looking = false;
    private float slerpTime = 1.0f; // Time to complete the slerp
    private float currentSlerpTime;

    void Start()
    {
        StartCoroutine(ToggleBoolEvery5Seconds());
        OriginalLookRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (looking)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation((Player.transform.position + LookUpDistance) - transform.position);
            targetRotation = lookAtRotation * headRotation;
        }
        else
        {
            targetRotation = OriginalLookRotation;
        }

        currentSlerpTime += Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentSlerpTime / slerpTime);
    }

    IEnumerator ToggleBoolEvery5Seconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            looking = !looking;
            currentSlerpTime = 0; // Reset slerp time
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class SpikeScript : MonoBehaviour
{
    public Transform transformA;
    public Transform transformB;
    public VisualEffect HitEffect;
    public float moveSpeed = 2.0f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool movingToB = true;
    public float waitTime = .5f;
    private float waitTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        targetPosition = transformB.position;
        moveSpeed = Random.Range(100, 300);
    }

    private void FixedUpdate()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        float step = moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, step);
        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, targetPosition) < 0.01f)
        {
            HitEffect.Play(); 

            if (movingToB)
            {
                targetPosition = transformA.position;
            }
            else
            {
                targetPosition = transformB.position;
            }
            movingToB = !movingToB;
            waitTimer = waitTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("StickWizard"))
        {
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(1); // kill that mfer :D

        }
    }
}

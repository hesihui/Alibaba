using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class FlyingRobotEnemyScript : Unit
{
    public GameObject BulletPrefab;
    public Transform BulletSpawn1;
    public Transform BulletSpawn2;
    public VisualEffect BulletEffect1;
    public VisualEffect BulletEffect2;

    public float oscillationAmplitude = 1f; 
    public float oscillationFrequency = 1f; 
    public float moveSpeed = 5f; 
    public float rotationSpeed = 5f; 
    public float orbitRadius = 5f; 
    public float orbitSpeed = 1f;

    public float attackSpeed = 2;

    private Vector3 initialPosition;
    private float startTime;
    private float orbitAngle;
    private void Update()
    {
        float timeElapsed = Time.time - startTime;
        float oscillationOffset = Mathf.Sin(timeElapsed * oscillationFrequency) * oscillationAmplitude;
        float verticalOffset = Mathf.Cos(timeElapsed * oscillationFrequency) * oscillationAmplitude*.1f;

        orbitAngle += orbitSpeed * Time.fixedDeltaTime;
        float orbitX = Mathf.Cos(orbitAngle) * orbitRadius;
        float orbitZ = Mathf.Sin(orbitAngle) * orbitRadius;
        Vector3 orbitOffset = new Vector3(orbitX, 0, orbitZ);

        Vector3 directionToTarget = (target.position - transform.position).normalized;


        Vector3 targetPosition = target.position + orbitOffset + directionToTarget * oscillationOffset;
        targetPosition.y += verticalOffset;


        directionToTarget.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        Quaternion adjustedRotation = lookRotation * Quaternion.Euler(-90, 0, 0);
        Quaternion newRotation = Quaternion.Slerp(rb.rotation, adjustedRotation, rotationSpeed * Time.fixedDeltaTime);

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(newRotation);



    }

  
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ShootEmUp());
    }

    private bool useFirstSpawn = true; 

    IEnumerator ShootEmUp()
    {
        yield return new WaitForSeconds(Random.Range(attackSpeed/4, attackSpeed));

        if (useFirstSpawn) BulletEffect1.Play();
        else BulletEffect2.Play();

        yield return new WaitForSeconds(.1f);

        Player.SoundEffectSource.pitch = Random.Range(.8f, 1.2f);
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/FlyingRobot/laserShoot"), 1);
        Transform chosenSpawn = useFirstSpawn ? BulletSpawn1 : BulletSpawn2;
        GameObject Bullet = Instantiate(BulletPrefab, chosenSpawn.position, transform.rotation);
        Bullet.GetComponent<Rigidbody>().AddForce(-transform.up * 150, ForceMode.Impulse);
        GameObject.Destroy(Bullet, 3);
        


        useFirstSpawn = !useFirstSpawn;

        StartCoroutine(ShootEmUp());
    }
}

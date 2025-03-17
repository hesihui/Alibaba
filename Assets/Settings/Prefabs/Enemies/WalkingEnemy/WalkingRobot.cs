using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingRobot : Unit
{
    public Animator A;
    public float damage = 1;

    new void Start()
    {
        base.Start();
        StartCoroutine(Jumping());
    }

    bool leg = false;
    IEnumerator Jumping()
    {
        yield return new WaitForSeconds(.9f);
        rb.velocity = new Vector3(rb.velocity.x, 50, rb.velocity.z);
        yield return new WaitForSeconds(.5f);
        leg = !leg;
        if (leg)
        {
            A.SetFloat("SpeedMultiplier", 1f);
            A.Play("LeftLegUp", 0, 0f);
        }
        else
        {
            A.SetFloat("SpeedMultiplier", -1f);
            A.Play("LeftLegUp", 0, 1f);
        }
        rb.velocity = new Vector3(rb.velocity.x, -50, rb.velocity.z);
        StartCoroutine(Jumping());
    }


    //This method in the base class destroys itself above 40, because this enemy jumps we don't want it to destroy itself at 40
    public override void FixedUpdate()
    {
        if ((transform.position.y < -100 || transform.position.y > 100) && LesserEnemy)
        {
            Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), .7f);


            GameObject.Destroy(Instantiate(EnemyDeathEffect, transform.position, transform.rotation), 2);

            Level.EnemyCount--;

            if (Level.EnemyCount <= 0)
            {
                //If there's no enemies, open the boss door(if it exists)
                GameObject bossDoor = GameObject.Find("BossDoor.2024.08.04(Clone)");
                if (bossDoor != null)
                {
                    bossDoor = bossDoor.transform.Find("InnerDoor").gameObject;
                    Renderer[] renderers = bossDoor.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        Material[] materials = renderer.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            materials[i] = Player.DissolveBossDoorMaterial;
                        }
                        renderer.materials = materials;
                    }

                }
                Transform T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;
                Transform T = T2.Find(Player.CurrentRoom.RoomNumber.ToString());
                Transform Doors = T.Find("Doors");
                Player.CurrentRoom.Cleared = true;
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Door");
                Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/LatchDoor"), 3);
                Animator A;
                foreach (GameObject g in objectsWithTag)
                {
                    if (g.TryGetComponent<Animator>(out A))
                        A.enabled = true;
                }
                if (transform.parent != null)
                    if (transform.parent.name == "Enemies")
                        GameObject.Destroy(transform.parent.gameObject);
            }

            gameObject.SetActive(false);
        }
    }

    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;

        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        Quaternion newTR = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTR, Time.deltaTime * TurnSpeed);

        // Set the velocity for forward movement
        rb.velocity = new Vector3(transform.forward.x * speed, rb.velocity.y, transform.forward.z * speed);

        // Smoothly reduce lateral velocity to 0 over time
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), Time.deltaTime / 4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("StickWizard"))
        {
            HitPlayer = true;
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(damage);
            Invoke(nameof(GivePlayerASecond), .2f);
        }
        if (collision.collider.tag == "Enemy")
        {
            HitPlayer = true;
            collision.collider.transform.GetComponent<Rigidbody>().AddForce((collision.collider.transform.position - transform.position) * Player.EnemyKnockBack*25, ForceMode.Impulse);
            Invoke(nameof(GivePlayerASecond), .2f);
        }
    }
}

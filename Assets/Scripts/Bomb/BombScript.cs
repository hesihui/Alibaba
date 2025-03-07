using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    private bool ableToDropBomb = true;
    private bool isBombActive = false;



    public void WaitForBombDrop()
    {
        ableToDropBomb = true;
    }

    public IEnumerator BombCountdown(float seconds, GameObject bomb)
    {

        bomb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Fuse"), 3);

        float minSpeed = 1;
        float maxSpeed = 20;

        float TTime = 0;

        for (float i = 0; i <= seconds * 100; i++)
        {
            if(i>25) bomb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

            Material M1 = bomb.GetComponent<MeshRenderer>().materials[0];
            Material M2 = bomb.GetComponent<MeshRenderer>().materials[1];

            TTime += Time.deltaTime;

            float angle = TTime * Mathf.Lerp(minSpeed, maxSpeed, i / seconds / 100);
            float sinValue = Mathf.Sin(angle);

            M1.SetFloat("_FlashSpeed", sinValue);
            M2.SetFloat("_FlashSpeed", sinValue);

     
            yield return new WaitForSeconds(.01f);
        }
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/BombExplosion"), 5);
        GameObject.Destroy(Instantiate(Player.BombExplosion, bomb.transform.position, Quaternion.identity), 3);

        float explosionsize = 30;

        Collider[] colliders = Physics.OverlapSphere(bomb.transform.position, explosionsize);

        //GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //debugSphere.transform.position = bomb.transform.position;
        //debugSphere.transform.localScale = new Vector3(explosionsize, explosionsize, explosionsize); // Adjust size to match the explosion radius
        //debugSphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.2f); // Semi-transparent red
        //GameObject.Destroy(debugSphere, 3); // Destroy after 3 seconds

        foreach (Collider c in colliders)
        {

            Unit U;

            if (c.TryGetComponent<Unit>(out U))
            {
                Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), .7f);
                U.TriggerMaterialChange();
                U.Health -= 5;



                if (U.Health <= 0)
                {
                    float force = 50f;
                    foreach (Drop d in Player.AvailableDrops)
                    {
                        int limiter = 0;
                        while (Random.value < d.DropChance)
                        {
                            limiter++;
                            GameObject G = Instantiate(d.Prefab, U.transform.position + new Vector3(0, 10, 0), Quaternion.identity);
                            G.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-force, force), Random.Range(30, 50), Random.Range(-force, force)), ForceMode.Impulse);
                            Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinNoise"), 1);
                            if (limiter > 10) break;
                        }
                    }
                    GameObject.Destroy(Instantiate(U.EnemyDeathEffect, U.transform.position, U.transform.rotation), 2);
                    Level.EnemyCount--;
                    U.gameObject.SetActive(false);
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
                        if (c.transform.parent != null)
                            if (c.transform.parent.name == "Enemies")
                                GameObject.Destroy(c.transform.parent.gameObject);
                    }
                }
                else
                {
                    U.HitPlayer = true;
                    Rigidbody rb;
                    c.transform.TryGetComponent<Rigidbody>(out rb);
                    if (rb != null)
                    {

                        Vector3 pushDirection = (U.transform.position - c.transform.position).normalized;
                        pushDirection.y = 0;
                        rb.velocity = pushDirection * Player.EnemyKnockBack * 12;
                        CoroutineManager.Instance.StartManagedCoroutine(ResetHitPlayer(U));
                    }
                }
            }

            PlayerMovement pm;
            if(c.TryGetComponent<PlayerMovement>(out pm))
            {
                ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - bomb.transform.position) * Player.KnockBack);
                HeartScript.TakeDamage(1);
            }

            RockScript rs;
            if(c.TryGetComponent<RockScript>(out rs))
            {
                rs.BlowUp();
            }
        
        }

        GameObject.Destroy(bomb);

    }

    private IEnumerator ResetHitPlayer(Unit U)
    {
        if (!U.HitPlayer) yield break;
        yield return new WaitForSeconds(0.4f);

        U.HitPlayer = false;
    }

    void Update()
    {
        if (Input.GetButtonUp("Bomb") && ableToDropBomb && Player.Bombz >0)
        {
            Player.Bombz--;
            Player.txtBombs.text = $"X{Player.Bombz:D2}";
            ableToDropBomb = false;
            StartCoroutine(BombCountdown(2, Instantiate(Player.Bomb, transform.position + new Vector3(0, 1, 0), Player.Bomb.transform.rotation)));
            Invoke(nameof(WaitForBombDrop), .1f);
        }
        
    }
}

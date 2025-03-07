using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{

  

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("Floor") || collision.collider.name.Contains("Attack")) return;
  
        GameObject.Destroy(Instantiate(Player.AttackExplosion, transform.position, Quaternion.identity), 3);
        Unit U;

        if (collision.collider.TryGetComponent<Unit>(out U))
        {
            Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), .7f);
            U.TriggerMaterialChange(collision.GetContact(0).point);
            U.Health -= (Player.Damage * Player.DamageMultiplier);

            

            if (U.Health <= 0)
            {
                float force = 50f;
                foreach(Drop d in Player.AvailableDrops)
                {
                    int limiter = 0;
                    while(Random.value < d.DropChance)
                    {
                        limiter++;
                        GameObject G = Instantiate(d.Prefab, U.transform.position + new Vector3(0,10,0), Quaternion.identity);
                        G.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-force, force), Random.Range(30, 50), Random.Range(-force, force)), ForceMode.Impulse);
                        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinNoise"), 1);
                        if (limiter > 10) break;
                    }
                }
                GameObject.Destroy(Instantiate(U.EnemyDeathEffect,U.transform.position,U.transform.rotation),2);
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
                    if (collision.transform.parent != null)
                        if (collision.transform.parent.name == "Enemies")
                            GameObject.Destroy(collision.transform.parent.gameObject);
                }
            }
            else
            {
                U.HitPlayer = true;
                Rigidbody rb;
                collision.collider.transform.TryGetComponent<Rigidbody>(out rb);
                if (rb != null)
                {

                    Vector3 pushDirection = (U.transform.position - collision.GetContact(0).point).normalized;
                    pushDirection.y = 0;
                    rb.velocity = pushDirection * Player.EnemyKnockBack * 12;
                    CoroutineManager.Instance.StartManagedCoroutine(ResetHitPlayer(U));
                }
            }
        }

        Invoke(nameof(DelayDestroy), .1f);
    }

    public void DelayDestroy()
    {
        GameObject.Destroy(gameObject);
    }
    
    private IEnumerator ResetHitPlayer(Unit U)
    {
        if (!U.HitPlayer) yield break;
        yield return new WaitForSeconds(0.4f);
        
        U.HitPlayer = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CoroutineManager");
                instance = go.AddComponent<CoroutineManager>();
            }
            return instance;
        }
    }

    public void StartManagedCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}

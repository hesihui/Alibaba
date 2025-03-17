using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class HeartScript : MonoBehaviour
{

    public static void DrawHeart(Sprite Type, int num)
    {
        GameObject Heart = new GameObject("Heart");
        Image HeartImage = Heart.AddComponent<Image>();
        HeartImage.sprite = Type;
        RectTransform rectTransform = Heart.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10, Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10);
        Animator animator = Heart.AddComponent<Animator>();
        animator.runtimeAnimatorController = Player.HeartAnimator;


        float XPos = 0;
        float YPos = -5;
        if (num <= 9)
        {
             XPos = num * Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
        }
        else
        {
            XPos = (num-10) * Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
            YPos = -5 - Player.HeartPanel.GetComponent<RectTransform>().sizeDelta.x / 10;
        }
        rectTransform.position = new Vector2(XPos, YPos);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax= new Vector2(0, 1);
        HeartImage.transform.SetParent(Player.HeartPanel.transform, false);

    }


    public static void DrawHearts()
    {

        for (int i = 0; i <= Player.Health - 1; i++) 
        {
            DrawHeart(Player.FullHeart, i);
        }

        if (Player.Health % 1 != 0)
        {
            DrawHeart(Player.HalfHeart, (int)Player.Health);
        }

        for (int i = (int)Player.Health; i <= Player.MaxHealth-1; ++i)
        {
            DrawHeart(Player.EmptyHeart, (int)i);
        }

    }

    public static IEnumerator Uninvincible()
    {
        yield return new WaitForSeconds(.5f);
        Player.Invincible = false;

    }


    public static void ApplyRigidBodyAndForce(Transform parent)
    {
        foreach (Transform child in parent)
        {

            if (child.gameObject.name == "DeathEffect") continue;

            if(child.gameObject.name.Contains("OuterLeftArm")) child.gameObject.SetActive(false);

            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                mc.convex = true;
                rb = child.gameObject.AddComponent<Rigidbody>();
            }

          
            Vector3 randomForce = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * 50;

            rb.AddForce(randomForce, ForceMode.Impulse);


            if (child.position.y < 1)
            {
                Vector3 newPos = child.position;
                newPos.y = 5;
                child.position = newPos;
            }

          
            ApplyRigidBodyAndForce(child);
        }
    }

    public static IEnumerator Die(Transform transform)
    {
        Player.DamagePanel.SetActive(true);
        yield return new WaitForSeconds(0f);

        for (int i = 0; i < Player.HeartPanel.transform.childCount; ++i)
        {
            GameObject.Destroy(Player.HeartPanel.transform.GetChild(i).gameObject);
        }

        Player.animator.StopPlayback();
        Player.State = "Dead";
        Player.Invincible = true;
        Destroy(Player.transform.GetComponent<Animator>());
        Destroy(Player.transform.GetComponent<PlayerMovement>());
        Destroy(Player.transform.GetComponent<PlayerAttack>());
        Destroy(Player.transform.GetComponent<ChangeRooms>());
        Destroy(Player.transform.GetComponent<Rigidbody>());
        Destroy(Player.transform.GetComponent<CharacterController>());
        


        ApplyRigidBodyAndForce(transform);

        GameObject.Find("DeathEffect").GetComponent<VisualEffect>().Play();
        Player.Audio.Stop();
        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/DeathSound"),3);
        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Die"), 3);
        yield return new WaitForSeconds(.5f);
        Player.DamagePanel.SetActive(false);

    }

    public static IEnumerator WaitAndRedrawHearts()
    {
        Player.DamagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        Player.DamagePanel.SetActive(false);

        yield return new WaitForSeconds(.4f);
        for (int i = 0; i < Player.HeartPanel.transform.childCount; ++i)
        {
            GameObject.Destroy(Player.HeartPanel.transform.GetChild(i).gameObject);
        }
        DrawHearts();
   
    }

    public static void TakeDamage(float damage)
    {
        if (!Player.Invincible)
        {   Player.Invincible = true;
            CoroutineManager.Instance.StartCoroutine(Uninvincible());
            Player.SoundEffectSource.pitch = Random.Range(.8f, 1.5f);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), 1);

            if (Player.Armor > 0)
            {
                Player.DamageBlockedByArmor += damage;
                if (Player.DamageBlockedByArmor > Player.Armor)
                {
                    HeartScript.TakeDamage(Player.DamageBlockedByArmor - Player.Armor);
                    Player.DamageBlockedByArmor = 0;
                }
                else
                {
                    return;
                }
            }

            if ((Player.Health - damage) <= 0 && Player.State != "Dead")
            {
                CoroutineManager.Instance.StartCoroutine(Die(GameObject.Find("StickWizard.2024.06.01").transform));
                return;
            }
            for (int i = 1; i <= damage; ++i)
            {
                (Instantiate(Player.HeartPanel.transform.GetChild((int)Player.Health - i),
                    Player.HeartPanel.transform)).GetComponent<Animator>().Play("HeartAnimation");
                Player.HeartPanel.transform.GetChild((int)Player.Health - i).GetComponent<Image>().sprite = Player.EmptyHeart;
            }
            Player.Health -= damage;
            if(Player.Health > 0) CoroutineManager.Instance.StartCoroutine(WaitAndRedrawHearts());
        }
    }

    void Start()
    {
        DrawHearts();

        
    }


   



    
}

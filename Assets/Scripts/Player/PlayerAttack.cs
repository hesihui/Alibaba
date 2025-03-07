using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAttack : MonoBehaviour
{
    public VisualEffect AttackEffect;
    public Transform AttackSpawnSpot;

    float LastAttackTime = 0;

    public void FinishAttacking()
    {
        if (Input.GetButton("Fire1"))
            Player.animator.Play("Attack");
        else
        {
            AttackEffect.Stop();
            AttackEffect.Reinit();
            Player.animator.Play("Idle");
            Player.State = "Idle";
        }
    }

    public void Attack()
    {
        AttackEffect.Stop();
        AttackEffect.Reinit();
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/PlayerAttackSounds/"+ Random.Range(1,7).ToString()),.5f);


        GameObject g = Instantiate(Player.Attack, new Vector3(AttackSpawnSpot.position.x,10, AttackSpawnSpot.position.z), Quaternion.Euler(0,Player.Attack.transform.rotation.y,0));
        g.GetComponent<Rigidbody>().velocity =  transform.forward * 70;
        g.transform.localScale = g.transform.localScale * Player.AttackSize;

        AttackEffect.Play();
        GameObject.Destroy(g,5);
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Player.State == "Idle")
        {
            Player.State = "Attacking";
            string AttackNumber = Random.Range(1, 5).ToString();

            LastAttackTime = Time.time;
            Player.animator.Play("Attack");

        }

        if (Input.GetButton("Fire1") && Player.State == "Attacking")
        {
            if(Time.time > LastAttackTime+.2f)
            {
                Player.State = "Idle";
            }

        }
    }
}

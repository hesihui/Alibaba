using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieScriptTutorial : Unit
{
    public float damage = 1f;
    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;
        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        Quaternion newTR = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTR, Time.deltaTime * TurnSpeed);
        Vector3 translation = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime/4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name.Contains("StickWizard"))
        {
            HitPlayer = true;
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(damage);
            Invoke(nameof(GivePlayerASecond), .2f);
        }
        if (collision.collider.tag == "Enemy")
        {
            HitPlayer = true;           
            collision.collider.transform.GetComponent<Rigidbody>().AddForce((collision.collider.transform.position - transform.position) * Player.EnemyKnockBack, ForceMode.Impulse);
            Invoke(nameof(GivePlayerASecond), .2f);
        }
    }
   
}

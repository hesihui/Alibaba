using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EProjectileScript : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name.Contains("StickWizard"))
        {
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(damage);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), 1);
        }

        if(collision.transform.tag != "Enemy")
        GameObject.Destroy(gameObject);
    }
}

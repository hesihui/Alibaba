using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Contains("StickWizard"))
        {

            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(1);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), 1);
        }
    }
}

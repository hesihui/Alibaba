using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpiderScript : Unit
{
    public Animator A;
    public Rigidbody R;
    public MeshRenderer MR;
    public float groundDistance = 0.2f;
    private bool isGrounded;
    public VisualEffect SpiderJump;
    private float WalkTime = 0;


    new private void Start()
    {
        base.Start();

        float size = Random.Range(1f, 1.3f);
        transform.localScale= new Vector3(size, size, size);
        if (MR.materials.Length >= 3)
        {
            Material[] materials = MR.materials;
            Material newMaterial = new Material(materials[2]);
            Color emissionColor = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
            newMaterial.color = emissionColor;
            newMaterial.EnableKeyword("_EMISSION");
            float emissionIntensity = 50.0f; 
            newMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            materials[2] = newMaterial;
            MR.materials = materials;
        }
        StartCoroutine(JUMP_EM());
    }

  


    public IEnumerator JUMP_EM()
    {
        yield return new WaitForSeconds(Random.Range(3, 10));

        if (Physics.Raycast(transform.position, Vector3.down, groundDistance))
        {
            HitPlayer = true;
            SpiderJump.Play();
            Player.SoundEffectSource.pitch = Random.Range(0.9f, 1.1f);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Spider/Clank"), 1);
            A.Play("Jump");
            R.AddForce(transform.forward * Random.Range(100, 200) + new Vector3(0, Random.Range(0, 50), 0), ForceMode.Impulse);
            StartCoroutine(ResetHitPlayer(this, 1f));
        }
        StartCoroutine(JUMP_EM());
    }


    float nexttime = 2;
    override public void FollowPathAction(Path path, int pathIndex)
    {
        if (HitPlayer) return;
        if (!Physics.Raycast(transform.position, Vector3.down, groundDistance)) return;
        Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
        Quaternion newTR = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newTR, Time.deltaTime * TurnSpeed);
        Vector3 translation = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);


        if (Time.time >= nexttime)
        {
            Player.SoundEffectSource.pitch = 1f;
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Spider/SpiderWalk"), 1);
            nexttime = Time.time + 1.16f;
        }

         rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime/4);

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("StickWizard"))
        {
            HitPlayer = true;

            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            HeartScript.TakeDamage(1);


            Invoke(nameof(GivePlayerASecond), .2f);
        }
        if (collision.collider.tag=="Enemy")
        {
            collision.collider.transform.GetComponent<Rigidbody>().AddForce((collision.collider.transform.position - transform.position) * Player.EnemyKnockBack, ForceMode.Impulse);
            Unit U;
            collision.collider.TryGetComponent<Unit>(out U);
            U.HitPlayer = true;
            StartCoroutine(ResetHitPlayer(U));
        }
    }

    private IEnumerator ResetHitPlayer(Unit U, float time = .2f)
    {
        yield return new WaitForSeconds(time/2);
        SpiderJump.Stop();
        yield return new WaitForSeconds(time/2);
        A.Play("Run");
        U.HitPlayer = false;
        
   
    }
}

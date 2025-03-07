using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class BossTrainScript : Unit
{
    bool starting = true;
    public GameObject Attack;
    public float turnspeed = 50;
    Animator A;
    List<GameObject> AllAttacks = new List<GameObject>();
    public VisualEffect Door;
    public VisualEffect Smoke1;
    public VisualEffect Smoke2;
    public VisualEffect Smoke3;

    public Light DL;

    public Volume globalVolume;
     UnityEngine.Rendering.Universal.ChromaticAberration CA;
    UnityEngine.Rendering.Universal.LiftGammaGain LGG;

    public GameObject FinalCube;
    new void Start()
    {
        base.Start();

       
        globalVolume.profile.TryGet(out CA);
        globalVolume.profile.TryGet(out LGG);

        StartCoroutine(DelayedNoise());
        StartCoroutine(MoveAllAttacks());
        A = transform.parent.GetComponent<Animator>();
        
    }

    public IEnumerator ResetLight()
    {
        float duration = 2f;
        float elapsedTime = 0.0f;
        elapsedTime = 0.0f;
        float startIntensity = DL.intensity;
        float startChromeIntensity = CA.intensity.value;
        float startLGGIntensity = LGG.lift.value.w;

        duration = .5f;
        while (elapsedTime < duration)
        {
            DL.intensity = Mathf.Lerp(startIntensity, 1, elapsedTime / duration);
            CA.intensity.value = Mathf.Lerp(startChromeIntensity, 0, elapsedTime / duration);
            LGG.lift.value = new Vector4(Mathf.Lerp(0, .8f, elapsedTime / duration), Mathf.Lerp(0, .8f, elapsedTime / duration), 1.0f, Mathf.Lerp(startLGGIntensity, -.05f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final intensity is exactly 1
        DL.intensity = 1;
        CA.intensity.value = 0;

        
        
    }

    public IEnumerator DelayedNoise()
    {
        LGG.active = true;
        LGG.lift.value = new Vector4(0.0f, 0.0f, 1.0f, -.6f); 
        
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/BossWHOOSH"), 1);
        float duration = 5f;
        float elapsedTime = 0.0f;
        float currentIntensity = DL.intensity;
        float targetIntensity = Random.Range(0f, 1.5f);
        float cycleTime = .4f; 
        float cycleElapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            if (cycleElapsedTime >= cycleTime)
            {
                currentIntensity = targetIntensity;
                targetIntensity = Random.Range(0.4f, 5.1f);
                cycleElapsedTime = 0.0f;
            }

            DL.intensity = Mathf.Lerp(currentIntensity, targetIntensity, cycleElapsedTime / cycleTime);
            CA.intensity.value = DL.intensity*3;
            LGG.lift.value = new Vector4(0.0f, 0.0f, 1.0f, Mathf.Lerp(-.6f, -.1f,DL.intensity/1.5f));
            cycleElapsedTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ResetLight());
       
        A.enabled = false;
        starting = false;
        Smoke1.Play();
        Smoke2.Play();
        Smoke3.Play();
        yield return new WaitForSeconds(1);
        StartCoroutine(DoAttack());
    }

    public IEnumerator MoveAllAttacks()
    {
        yield return new WaitForSeconds(Random.Range(3, 6));
        foreach(GameObject attack in AllAttacks)
        {

            attack.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5,6),0,0) * 10, ForceMode.Impulse);
        }
        StartCoroutine(MoveAllAttacks());

    }

    public float lungeDistance = 50f;
    public float lungeTime = 0.2f;
    public float returnTime = 1f;

    public IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(Random.Range(1,3));

        Vector3 startPosition = transform.position;
        Vector3 lungePosition = startPosition + transform.forward * lungeDistance;

        float elapsedTime = 0f;
        while (elapsedTime < lungeTime)
        {
            transform.position = Vector3.Lerp(startPosition, lungePosition, elapsedTime / lungeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = lungePosition;
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/ElectricAttack"), 4);

        //attack here
        for (int i = 0; i <= 3; ++i)
        {
            GameObject attk = Instantiate(Attack, (transform.position + transform.forward * 20) + new Vector3(0, 5, 0), Quaternion.identity);
            float size = Random.Range(3, 4);
            attk.transform.localScale = new Vector3(size, size, size);
            Vector3 randomSpread = Vector3.right * Random.Range(-1.0f, 1.0f);
            Vector3 randomUpDown = Vector3.up * Random.Range(-0.5f, 0.5f);
            Vector3 forceDirection = (transform.forward + Vector3.right * (i - 1) + randomSpread + randomUpDown).normalized;
            float randomForce = Random.Range(50, 100);
            attk.GetComponent<Rigidbody>().AddForce(forceDirection * randomForce, ForceMode.Impulse);
            AllAttacks.Add(attk);
            GameObject.Destroy(attk, 100);

        }

        elapsedTime = 0f;
        while (elapsedTime < returnTime)
        {
            transform.position = Vector3.Lerp(lungePosition, startPosition, elapsedTime / returnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;

        StartCoroutine(DoAttack());
    }
    private void Update()
    {
        if (!starting)
        {
            Vector3 direction = Player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation  = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnspeed);
        }
    }

    public IEnumerator DieBossDieeeeeeeeeee(List<GameObject> attacks)
    {
        FinalCube.SetActive(true);

        while (AllAttacks.Count > 0)
        {
            for (int i = attacks.Count - 1; i >= 0; i--)
            {
                GameObject obj = attacks[i];
                if (obj == null) continue;

                obj.transform.position = Vector3.MoveTowards(obj.transform.position, FinalCube.transform.position, 50 * Time.deltaTime);

                if (Vector3.Distance(obj.transform.position, FinalCube.transform.position) < 5f)
                {
                    Destroy(obj);
                    attacks.RemoveAt(i);
                    FinalCube.transform.localScale += new Vector3(.5f, .5f, .5f);
                    FinalCube.GetComponent<Renderer>().material.color = FinalCube.GetComponent<Renderer>().material.color * 1.1f;
                    Player.SoundEffectSource.pitch += .1f;
                    Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/ElectricAttack"), 4);
                }
            }

            
            yield return null;

        }

        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/BossWHOOSH"), 2);

        while (true)
        {
            FinalCube.transform.position = Vector3.MoveTowards(FinalCube.transform.position, new Vector3(0, 5, 110), 150 * Time.deltaTime);
            if (Vector3.Distance(FinalCube.transform.position, new Vector3(0, 5, 110)) < 1f) break;

            yield return null;
        }


        while (true)
        {
            FinalCube.GetComponent<Renderer>().material.color = FinalCube.GetComponent<Renderer>().material.color / 1.3f;
            FinalCube.transform.localScale = new Vector3(
        Mathf.Max(0, FinalCube.transform.localScale.x - 0.5f),
        Mathf.Max(0, FinalCube.transform.localScale.y - 0.5f),
        Mathf.Max(0, FinalCube.transform.localScale.z - 0.5f)
    );
            if (FinalCube.GetComponent<Renderer>().material.color.a <= .01) break;
            yield return null;
        }

        Player.Audio.Stop();
        Player.Audio.clip = Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/PostBossMusic");
        Player.Audio.Play();
        Player.SoundEffectSource.pitch = 1f;
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/Bosses/1/LongWhoosh"), 1);

        FinalCube.SetActive(false);
        LGG.active = false;
        //GameObject.Destroy(FinalCube);
        Door.Play();

    }

    private void OnDisable()
    {
        CoroutineManager.Instance.StartCoroutine(DieBossDieeeeeeeeeee(AllAttacks));
    }

}

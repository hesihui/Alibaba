using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class WallBlasterScript : MonoBehaviour
{
    public Material greenMaterial, yellowMaterial, redMaterial, BrimstoneMaterial;
    public float emissionIntensity = 5f;
    public float delay = 1f;
    public VisualEffect Brimstone;
    public float radius = 1f;
    public float length = 10f;
    public LayerMask layerMask;

    private bool Activated = false;
    private Coroutine emissionCoroutine;

    void OnEnable()
    {
        // Start the coroutine when the script is enabled (i.e., when you enter the room)
        if (emissionCoroutine != null)
        {
            StopCoroutine(emissionCoroutine);
        }
        emissionCoroutine = StartCoroutine(HandleEmissions());
    }

    void OnDisable()
    {
        // Stop the coroutine when the script is disabled (i.e., when you leave the room)
        if (emissionCoroutine != null)
        {
            StopCoroutine(emissionCoroutine);
        }
        ResetState();
    }

    IEnumerator HandleEmissions()
    {
        while (true)
        {
            Activated = false;
            Brimstone.Stop();
            Brimstone.Reinit();
            SetEmission(BrimstoneMaterial, new Color(0, 0f, 0), 0f);
            SetEmission(greenMaterial, Color.green, emissionIntensity);
            Player.SoundEffectSource.pitch = 1f;
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/BeepWarning"), 1);
            yield return new WaitForSeconds(delay);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/BeepWarning"), 1);
            SetEmission(yellowMaterial, Color.yellow, emissionIntensity);
            yield return new WaitForSeconds(delay);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/BeepWarning"), 1);
            SetEmission(redMaterial, Color.red, emissionIntensity);
            yield return new WaitForSeconds(delay);
            SetEmission(greenMaterial, new Color(1, 0.5f, 0), 500f);
            SetEmission(yellowMaterial, new Color(1, 0.5f, 0), 500f);
            SetEmission(redMaterial, new Color(1, 0.5f, 0), 500f);
            Player.SoundEffectSource.pitch = 1.6f;
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/FlameThrower"), 3);
            yield return StartCoroutine(PerformAction());

            SetEmission(greenMaterial, Color.green, 0f);
            SetEmission(yellowMaterial, Color.yellow, 0f);
            SetEmission(redMaterial, Color.red, 0f);
            SetEmission(BrimstoneMaterial, new Color(0, 0f, 0), 0f);
            yield return new WaitForSeconds(.5f);
        }
    }

    void SetEmission(Material mat, Color color, float intensity)
    {
        if (mat != null)
        {
            Color finalColor = color * Mathf.LinearToGammaSpace(intensity);
            mat.SetColor("_EmissionColor", finalColor);
            mat.EnableKeyword("_EMISSION");
        }
    }

    IEnumerator PerformAction()
    {
        SetEmission(BrimstoneMaterial, new Color(1, 0.5f, 0), 500f);
        Brimstone.Play();
        Activated = true;

        // Perform the sphere cast continuously for the duration of the action
        float actionDuration = 3f; // Duration in seconds
        float elapsedTime = 0f;

        while (elapsedTime < actionDuration)
        {
            if (elapsedTime > .2f) PerformSphereCast();
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        Activated = false; // Reset Activated after performing the action
    }

    void PerformSphereCast()
    {
        Vector3 castCenter = transform.position;
        Vector3 castDirection = transform.forward;

        RaycastHit hit;
        bool hasHit = Physics.SphereCast(castCenter, radius, castDirection, out hit, length, layerMask);

        if (hasHit && hit.collider.name.Contains("StickWizard") && Activated)
        {
            HeartScript.TakeDamage(1);
        }
    }

    void ResetState()
    {
        Activated = false;
        Brimstone.Stop();
        SetEmission(greenMaterial, Color.green, 0f);
        SetEmission(yellowMaterial, Color.yellow, 0f);
        SetEmission(redMaterial, Color.red, 0f);
        SetEmission(BrimstoneMaterial, new Color(0, 0f, 0), 0f);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 castCenter = transform.position;
            Gizmos.color = Color.red;

            // Draw the SphereCast direction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(castCenter, castCenter + transform.forward * length);
        }
    }
}

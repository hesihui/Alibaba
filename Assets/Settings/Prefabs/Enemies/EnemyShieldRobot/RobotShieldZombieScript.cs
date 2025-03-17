using System.Collections;
using UnityEngine;

public class RobotShieldZombieScript : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public float hopHeight = 1.0f;
    public float hopFrequency = 2.0f;
    public float moveSpeed = 2.0f;
    public float stopDuration = 1.0f;
    public float cycleTime = 1f;
    public float brightnessMultiplier = 20f;
    public float distanceThreshold = 0.1f; 
    public float rotationSpeed = 180f; 
    public float hopDistanceThreshold = 1.0f; 

    public Material light1;
    public Material light2;
    public Material light3;

    private Vector3 targetPosition;
    private float originalYPosition;
    private bool isMoving = false;

    void OnEnable()
    {
        originalYPosition = transform.position.y;
        targetPosition = pointA.transform.position;

        StartCoroutine(UpdateEmissionsRoutine());
        StartCoroutine(MoveBetweenPoints());
        StartCoroutine(HopRoutine());
    }

    IEnumerator MoveBetweenPoints()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToPoint(targetPosition));
            yield return StartCoroutine(TurnAround(targetPosition == pointA.transform.position ? pointB.transform.position : pointA.transform.position));
            yield return new WaitForSeconds(stopDuration);
            targetPosition = targetPosition == pointA.transform.position ? pointB.transform.position : pointA.transform.position;
        }
    }

    IEnumerator MoveToPoint(Vector3 target)
    {
        isMoving = true;

        while (HorizontalDistance(transform.position, target) > distanceThreshold)
        {
            Vector3 direction = (target - transform.position).normalized;
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.z);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }

        transform.position = new Vector3(target.x, transform.position.y, target.z);
        isMoving = false;
    }

    IEnumerator TurnAround(Vector3 nextTarget)
    {
        Vector3 direction = (nextTarget - transform.position).normalized;
        Vector3 rotateDirection = new Vector3(direction.x, 0, direction.z);
        Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    bool alreadysounded = false;
    IEnumerator HopRoutine()
    {
        while (true)
        {
            if (isMoving)
            {
                float distanceToTarget = HorizontalDistance(transform.position, targetPosition);
                if (distanceToTarget > hopDistanceThreshold)
                {
                    if(Mathf.Sin(Time.time * hopFrequency) <-.8f && !alreadysounded)
                    {
                        alreadysounded = true;
                        Player.SoundEffectSource.pitch = 1f;
                        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/ShieldRobot/Bounce"), 1);
                    }
                    if (Mathf.Sin(Time.time * hopFrequency) >.8f)
                    {
                        alreadysounded = false;
                    }
                        float verticalPosition = Mathf.Sin(Time.time * hopFrequency) * hopHeight;
                    transform.position = new Vector3(transform.position.x, originalYPosition + verticalPosition, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
                }
            }
            yield return null;
        }
    }

    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
    }

    IEnumerator UpdateEmissionsRoutine()
    {
        while (true)
        {
            UpdateEmissions();
            yield return new WaitForSeconds(cycleTime);
        }
    }

    void UpdateEmissions()
    {
        float t = Mathf.PingPong(Time.time / cycleTime, 1f); 
        Color bright = Color.red * Mathf.LinearToGammaSpace(1.0f * brightnessMultiplier);
        Color mid = Color.white * Mathf.LinearToGammaSpace(0.5f * brightnessMultiplier);
        Color dim = Color.white * Mathf.LinearToGammaSpace(0.1f * brightnessMultiplier);

        light1.SetColor("_EmissionColor", Color.Lerp(mid, bright, t));
        light2.SetColor("_EmissionColor", Color.Lerp(dim, mid, t));
        light3.SetColor("_EmissionColor", Color.Lerp(bright, dim, t));
    }

    bool hitplayer = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("StickWizard") && !hitplayer)
        {
            hitplayer = true;
            Player.SoundEffectSource.pitch = 1.1f;
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Enemies/ShieldRobot/Bounce"), 2);
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - transform.position) * Player.KnockBack);
            Invoke(nameof(Wait), 1);
        }
    }
   
    public void Wait()
    {
        hitplayer = false;
    }

}

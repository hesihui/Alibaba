using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : EnemyDamageScript
{
    Grid g;

    public bool DrawPath = false;

    const float minPathUpdateTime = .05f;
    const float pathUpdateMoveThreshold = .5f;


    [Header("0: Chase Player\n" +
        "1: Random Path\n"+
        "99999: No Chasing")]
    public int MonsterMovementType = 0;
    
    public float Health = 3;

    public GameObject EnemyDeathEffect;
    public bool LesserEnemy = true;

    public Transform target;
    public float speed = 5f;
    public float turnDst = 5f;
    public float TurnSpeed = 3;
    Path path;
    [HideInInspector] public Rigidbody rb;

    public void DelayedStart()
    {
        switch (MonsterMovementType)
        {
            case 0: StartCoroutine(UpdatePath()); break;
            case 1: StartCoroutine(UpdatePathRandom()); break;
        }
    }

    public virtual void FixedUpdate()
    {
        //if the enemy falls it dies.
        if ((transform.position.y < -40 || transform.position.y > 30) && LesserEnemy)
        {
            Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Damage"), .7f);
         

            GameObject.Destroy(Instantiate(EnemyDeathEffect, transform.position, transform.rotation), 2);

            Level.EnemyCount--;

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
                if (transform.parent != null)
                    if (transform.parent.name == "Enemies")
                        GameObject.Destroy(transform.parent.gameObject);
            }

            gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
        g = GameObject.Find("A*").GetComponent<Grid>();
        Invoke(nameof(DelayedStart), .1f);
    }



    public bool HitPlayer = false;

    public void GivePlayerASecond()
    {
        HitPlayer = false;
    }

    public void OnPathFound(Vector3[] waypoints,bool pathSuccessful)
    {
        if (!pathSuccessful) return;
        if (HitPlayer) return;
        path = new Path(waypoints, transform.position, turnDst);
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");

    }

    IEnumerator UpdatePathRandom()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        while (true)
        {
            
            yield return new WaitForSeconds(.2f); 
            Node curN = g.NodeFromWorldPoint(transform.position);
            int max = g.grid.GetLength(0)-1;

            int newX = Mathf.Clamp(curN.gridX + Random.Range(-25, 25), 3, max-3);
            int newY = Mathf.Clamp(curN.gridY + Random.Range(-25, 25), 3, max - 3);

            int iterationNumber = 0;
            while (!g.grid[newX, newY].Walkable)
            {
                iterationNumber++;
                if (iterationNumber > 10) break;

                 newX = Mathf.Clamp(curN.gridX + Random.Range(-25, 25), 3, max - 3);
                 newY = Mathf.Clamp(curN.gridY + Random.Range(-25, 25), 3, max - 3);
                curN = g.grid[newX, newY];
            }

            Vector3 RPos = g.grid[newX, newY].WorldPosition;

            PathRequestManager.RequestPath(new PathRequest(transform.position, RPos, OnPathFound));

        }
    }


    IEnumerator UpdatePath()
    {
        

        if(Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.1f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;


        float counter = 0;

        while (true)
        {

            yield return new WaitForSeconds(minPathUpdateTime);

            if (HitPlayer) continue;
            ++counter;
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold || counter > 10)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
                counter = 0;
            }
        }
    }

    public virtual void FollowPathAction(Path path, int pathIndex)
    {
       
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        while (followingPath)
        {
            if (HitPlayer) {
                yield return null;
                continue; }

            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if(pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if(followingPath)
            {
                FollowPathAction(path, pathIndex);
                
            }
            yield return null;
        }
    }


    public void OnDrawGizmos()
    {
        if(path != null && DrawPath)
        {
            path.DrawWithGizmos();
        }
        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    private EnemyReferences enemyReferences;
    private float shootingDistance;
    private float pathUpdateDeadline;

    public Transform bulletSpawn;
    public Transform rayCastOrigin;
    public ParticleSystem muzzleflash;
    public int damage = 5;
    public float despawnRange = 12;

    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
    }

    // Start is called before the first frame update
    void Start()
    {
        NavMeshHit nearestNavmesh;
        if (NavMesh.SamplePosition(transform.position, out nearestNavmesh, 100, -1))
        {
            transform.position = nearestNavmesh.position;
        }
        GetComponent<NavMeshAgent>().enabled = true;

        shootingDistance = enemyReferences.navmeshAgent.stoppingDistance;

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            bool inRange = Vector3.Distance(transform.position, target.position) <= shootingDistance;
            if (inRange)
            {
                LookAtTarget();
                enemyReferences.animator.SetBool("Shooting", true);
            }
            else
                {
                    enemyReferences.animator.SetBool("Shooting", false);
                    if (enemyReferences.navmeshAgent.isOnNavMesh)
                    {
                        UpdatePath();
                    }
                    else
                    {
                        NavMeshHit nearestNavmesh;
                        if (NavMesh.SamplePosition(transform.position, out nearestNavmesh, 100, -1))
                        {
                            transform.position = nearestNavmesh.position;
                        }
                    }
                }
            
            
            if (Vector3.Distance(transform.position, target.position) > despawnRange)
            {
                Destroy(gameObject);
                print("Despawned enemy");
            }
        }
        enemyReferences.animator.SetFloat("Speed", enemyReferences.navmeshAgent.desiredVelocity.sqrMagnitude);

        if(Vector3.Distance(bulletSpawn.position, transform.position) > 10)
        {
            Destroy(gameObject);
            print("Removed invisible enemy");
        }
    }

    private void UpdatePath()
    {
        if(Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            enemyReferences.navmeshAgent.SetDestination(target.position);
        }
    }

    private void LookAtTarget()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }

    public void Shoot()
    {
        //Due to animation it is necessary to reference the bone's position at the end of the frame.
        StartCoroutine(SpawnAtEndOfFrame());
    }

    IEnumerator SpawnAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        ParticleSystem flash = Instantiate(muzzleflash, bulletSpawn.position, bulletSpawn.rotation);
        Destroy(flash, 1);

        RaycastHit hit;
        if (Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hit))
        {
            if (hit.transform.gameObject.layer == 6) //Player
            {
                target.GetComponent<PlayerStatus>().damage(damage);
            }
        }
    }
}

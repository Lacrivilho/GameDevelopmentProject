using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawn;
    public ParticleSystem muzzleflash;

    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 2f;

    public Camera viewCam;

    public LayerMask layerMask;

    public Vector3 aimingOffset;
    public float aimingDuration = 0.1f;
    private Vector3 initialGunPosition;
    private bool aiming = false;

    /* Update is called once per frame
    void Update()
    {
        //left mouse shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }*/

    void Start()
    {
        initialGunPosition = transform.localPosition;
    }

    public void FireWeapon()
    {
        ParticleSystem flashInstance = Instantiate(muzzleflash, bulletSpawn.position, bulletSpawn.rotation);
        Destroy(flashInstance, 2);

        RaycastHit hit;
        if(Physics.Raycast(viewCam.transform.position, viewCam.transform.forward, out hit, range, layerMask))
        {
            print(hit.transform.name);
            if (hit.transform.gameObject.layer == 7)
            {
                hit.transform.gameObject.GetComponentInParent<Animator>().enabled = false;
                hit.transform.gameObject.GetComponentInParent<NavMeshAgent>().enabled = false;
                hit.transform.gameObject.GetComponentInParent<EnemyController>().enabled = false;
                hit.transform.root.Find("Man").GetComponent<MeshCollider>().enabled = false;

                hit.transform.root.GetComponentInChildren<Rigidbody>().AddForceAtPosition(hit.point, viewCam.transform.forward * impactForce, ForceMode.Impulse);

                if (hit.transform.GetComponent<Rigidbody>())
                {
                    hit.transform.GetComponent<Rigidbody>().AddForce(viewCam.transform.forward * impactForce, ForceMode.Impulse);
                }
                else
                {
                    Transform[] children = hit.transform.root.GetComponentsInChildren<Transform>();
                    Transform closestChild = null;
                    float childDistance = float.MaxValue;

                    foreach (Transform child in children)
                    {
                        if (child.GetComponent<Rigidbody>()) {
                            if (Vector3.Distance(child.position, hit.point) < childDistance)
                            {
                                childDistance = Vector3.Distance(child.position, hit.point);
                                closestChild = child;
                            }
                        }
                    }

                    closestChild.GetComponent<Rigidbody>().AddForce(viewCam.transform.forward * impactForce, ForceMode.Impulse);
                }

                Destroy(hit.transform.gameObject.transform.root.gameObject, 5);
            }
        }
    }

    public void TransitionGunPosition(int aimInt)
    {
        bool aim = aimInt == 1;
        if(aim != aiming)
        {
            StartCoroutine(TransitionGunPositionNumerator(aim));
            aiming = aim;
        }
    }

    private IEnumerator TransitionGunPositionNumerator(bool aim)
    {
        float elapsedTime = 0f;
        Vector3 start = transform.localPosition;
        Vector3 end = aim ? initialGunPosition + aimingOffset : initialGunPosition;

        while (elapsedTime < aimingDuration)
        {
            transform.localPosition = Vector3.Lerp(start, end, elapsedTime / aimingDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;
    }

    public void unblockShootingRe()
    {
        PlayerMovement.unblockShooting();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 2f;

    public Camera viewCam;

    public LayerMask layerMask;

    /* Update is called once per frame
    void Update()
    {
        //left mouse shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }*/

    public void FireWeapon()
    {
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
}

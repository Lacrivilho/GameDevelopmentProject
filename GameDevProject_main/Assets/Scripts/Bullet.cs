using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Bullet(Clone)")
        {
            if (collision.gameObject.layer == 7)
            {
                collision.gameObject.GetComponentInParent<Animator>().enabled = false;
                collision.gameObject.GetComponentInParent<NavMeshAgent>().enabled = false;
                collision.gameObject.GetComponentInParent<EnemyController>().enabled = false;

                Destroy(collision.gameObject.transform.root.gameObject, 5);
            }

            Destroy(gameObject, 0.2f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) //Player
        {
            other.transform.root.GetComponent<PlayerStatus>().heal(healthAmount);

            Destroy(gameObject);
        }
    }
}

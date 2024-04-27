using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount;

    private void Awake()
    {
        ammoAmount = GameManager.Instance.ammoPackSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) //Player
        {
            other.transform.root.GetComponent<PlayerStatus>().reload(ammoAmount);

            Destroy(gameObject);
        }
    }
}

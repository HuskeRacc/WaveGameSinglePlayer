using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    PlayerHealth playerHealth;
    

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("health"))
        {
            Debug.Log("health picked up");
            playerHealth.health += 30f;
        }
    }
}

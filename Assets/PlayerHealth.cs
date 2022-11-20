using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    private void Update()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is Dead");
        //Restart scene?
        //Respawn & clear enemies and relevant stats?
    }

}

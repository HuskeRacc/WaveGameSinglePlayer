using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public int sceneToLoad = 1;

    private void Update()
    {
        if(health <= 0)
        {
            Die();
        }

        if (health > 100)
        {
            health = 100;
        }
    }

    void Die()
    {
        Debug.Log("Player is Dead");
        //Restart scene?
        //Respawn & clear enemies and relevant stats?
        SceneManager.LoadScene(sceneToLoad);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesAlive : MonoBehaviour
{
    public GameObject[] enemies;

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("enemy");
    }
}

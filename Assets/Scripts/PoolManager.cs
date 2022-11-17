using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //Object to instantiate
    public GameObject bulletHolePrefab;
    public int spawnCount;
    //List of objects that can spawn
    public List<GameObject> bulletHoleList;

    private void Start()
    {
        //spawn spawnCount bullets and add to list
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject bullet =  Instantiate(bulletHolePrefab) as GameObject;
            bulletHoleList.Add(bullet);
            bullet.transform.parent = this.transform;
            bullet.SetActive(false);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new();
    public List<GameObject> enemiesToSpawn = new();
    public int currWave;
    public int waveValue;

    public GameObject[] spawnLocations;
    public int waveDuration;
    float waveTimer;
    public float spawnInterval;
    public float spawnTimer;

    public List<GameObject> spawnedEnemies = new();

    private void Start()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("spawner");
        GenerateWave();
    }

    void IncreaseWave()
    {
            currWave++;
    }

    private void FixedUpdate()
    {
        if(spawnTimer <= 0)
        {
            //spawn enemy
            if(enemiesToSpawn.Count > 0)
            {
                Instantiate(enemiesToSpawn[0], spawnLocations[Random.Range(0,spawnLocations.Length)].transform.position,Quaternion.identity);
                enemiesToSpawn.RemoveAt(0);
                spawnTimer = spawnInterval;
            }
            else
            {
                waveTimer = 0;
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }
    }

    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();
        if(enemiesToSpawn.Count != 0)
        {
            spawnInterval = waveDuration / enemiesToSpawn.Count; // fixed time between enemies spawned
            waveTimer = waveDuration;        
        }
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while(waveValue > 0)
        {
            int randomEnemyId = Random.Range(0, enemies.Count);
            int randomEnemyCost = enemies[randomEnemyId].cost;

            if(waveValue - randomEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randomEnemyId].enemyPrefab);
                waveValue -= randomEnemyCost;
            } else if(waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}
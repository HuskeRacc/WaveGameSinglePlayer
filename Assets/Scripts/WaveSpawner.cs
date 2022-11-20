using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new();
    public List<GameObject> enemiesToSpawn = new();
    public List<GameObject> enemiesAlive = new();
    public int currWave;
    int waveValue;

    public GameObject[] spawnLocations;
    public int waveDuration;
    public float waveTimer;
    public float spawnInterval;
    public float spawnTimer;
    public float enemiesKilled;

    private void Start()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("spawner");
        GenerateWave();
    }

    private void Update()
    {
        if (enemiesToSpawn.Count == 0 && enemiesKilled >= enemiesAlive.Count)
        {
            IncreaseWave();
        }
    }

    private void FixedUpdate()
    {
        if(spawnTimer <= 0)
        {
            //spawn enemy
            if(enemiesToSpawn.Count > 0)
            {
                if (enemiesToSpawn[0] != null)
                {
                    Instantiate(enemiesToSpawn[0], spawnLocations[Random.Range(0, spawnLocations.Length)].transform.position, Quaternion.identity);
                    enemiesAlive.Add(enemiesToSpawn[0]);
                    enemiesToSpawn.Remove(enemiesToSpawn[0]);
                    spawnTimer = spawnInterval;
                }
                else
                {
                    Debug.LogError("EnemiesToSpawn[0] is empty!");
                }
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

    public void IncreaseWave()
    {
        currWave++;
        enemiesKilled = 0;
        enemiesAlive.Clear();
        GenerateWave();
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();

        while (waveValue > 0)
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
            else
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesKilled = 0;
        enemiesToSpawn = generatedEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public float enemySpawnDelay;
    public int maxEnemies = 7;

    private float lastSpawn = -10;

    private void Awake()
    {
        enemySpawnDelay = GameManager.Instance.enemySpawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCount = enemies.Length;

        if(lastSpawn + enemySpawnDelay < Time.time && enemyCount < maxEnemies)
        {
            // Choose a random spawn point
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Transform randomSpawnPoint = spawnPoints[randomSpawnIndex];

            GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().target = transform;

            lastSpawn = Time.time;
        }
    }
}

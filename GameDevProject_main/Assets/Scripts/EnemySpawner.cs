using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    public float enemySpawnDelay = 5;
    public int maxEnemies = 7;

    private float lastSpawn = -10;

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCount = enemies.Length;

        if(lastSpawn + enemySpawnDelay < Time.time && enemyCount < maxEnemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().target = transform;

            lastSpawn = Time.time;
        }
    }
}

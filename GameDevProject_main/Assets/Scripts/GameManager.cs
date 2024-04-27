using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // Public variables for game settings
    public bool spawnExit;
    public int exitSpawnDelay;

    public float healthPackChance;
    public float ammoPackChance;
    public int enemyDamage;
    public int ammoPackSize;
    public float enemySpawnDelay;

    public bool gameModeSurvive;

    public bool win;
    public bool gameOver = false;

    public int lastGameMode;

    public int killScore;

    // Property to access the singleton instance
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // If the instance is null, find the GameManager object in the scene
                instance = FindObjectOfType<GameManager>();

                // If no GameManager object exists in the scene, create a new one
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    // Prevent instantiation of GameManager through the constructor
    private GameManager() { }

    void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this instance as the singleton instance
        instance = this;

        // Keep this GameManager object persistent between scenes
        DontDestroyOnLoad(gameObject);
    }

    // Example method to update game settings
    public void setSEasy()
    {
        spawnExit = true;
        exitSpawnDelay = 30;
        healthPackChance = 1;
        ammoPackChance = 1;
        enemyDamage = 2;
        ammoPackSize = 20;
        enemySpawnDelay = 4;
        gameModeSurvive = true;
        lastGameMode = 0;
    }
    public void setSMid()
    {
        spawnExit = true;
        exitSpawnDelay = 100;
        healthPackChance = 0.7f;
        ammoPackChance = 0.7f;
        enemyDamage = 5;
        ammoPackSize = 15;
        enemySpawnDelay = 3;
        gameModeSurvive = true;
        lastGameMode = 1;
    }
    public void setSHard()
    {
        spawnExit = true;
        exitSpawnDelay = 250;
        healthPackChance = 0.3f;
        ammoPackChance = 0.5f;
        enemyDamage = 10;
        ammoPackSize = 10;
        enemySpawnDelay = 2;
        gameModeSurvive = true;
        lastGameMode = 2;
    }
    public void setHunt()
    {
        spawnExit = false;
        healthPackChance = 0.4f;
        ammoPackChance = 0.5f;
        enemyDamage = 15;
        ammoPackSize = 10;
        enemySpawnDelay = 1.5f;
        gameModeSurvive = false;
        lastGameMode = 3;
        killScore = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    
    [Header("Prefabs")]
    public GameObject basicEnemyPrefab;
    public GameObject shieldEnemyPrefab;

    private List<Enemy> aliveEnemies = new List<Enemy>();

    private Coroutine spawningCoroutine;
    private int currentWaveTotal;
    public int CurrentWaveTotal => currentWaveTotal;
    
    private void Awake()
    {
        GameManager.Instance.enemyManager = this;
    }

    private void OnEnable()
    {
        GameManager.WaveStart += SpawnWave;
        GameManager.GameOver += OnGameOver;
        GameManager.GoToMenu += OnGoToMenu;
    }

    private void OnDisable()
    {
        GameManager.WaveStart -= SpawnWave;
        GameManager.GameOver -= OnGameOver;
        GameManager.GoToMenu -= OnGoToMenu;
    }
    
    private void OnGameOver()
    {
        StopCoroutine(spawningCoroutine);
    }
    private void OnGoToMenu()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(aliveEnemies[i].gameObject);
        }
        aliveEnemies.Clear();
    }

    IEnumerator EndRound()
    {
        yield return new WaitForSeconds(0.5f);
        
        //temp: end game
        // GameManager.Instance.GameState = GameState.GameEnd;

        //TODO:
        //add UI or FX?
        GameManager.Instance.Player.health = GameManager.Instance.Player.maxHealth;
        GameManager.Instance.StartNextWave();
    }

    void SpawnWave(Wave wave)
    {
        spawningCoroutine = StartCoroutine(SpawnWaveCoroutine(wave));
        currentWaveTotal = wave.GetEnemyTotal();
    }
    
    //Loop through enemy sets that spawn together 
    IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        for (int setIndex = 0; setIndex < wave.enemySets.Count; setIndex++)
        {
            EnemySet set = wave.enemySets[setIndex];
            //spawn set of enemies
            for (int spawnPointIndex = 0; spawnPointIndex < set.enemies.Length; spawnPointIndex++)
            {
                SpawnEnemy(set.enemies[spawnPointIndex], spawnPointIndex);
                yield return new WaitForSeconds(0.1f); //spicy ;)
            }
            
            if (set.waitForAllEnemiesDead)
            {
                while (aliveEnemies.Count > 0)
                {
                    //may hang here if enemies don't get destroyed properly
                    yield return null; 
                }
            }
            else
            {
                yield return new WaitForSeconds(wave.timeBetweenSets);
            }
        }

        if (GameManager.Instance.Round < 3)
        {
            //wait for all enemies to be dead
            while (aliveEnemies.Count > 0)
            {
                yield return null;
            }
            StartCoroutine(EndRound());
        }
        else
        {
            SpawnWave(wave);
        }
    }

    void SpawnEnemy(EnemyType enemyType, int spawnPoint)
    {
        if (enemyType == EnemyType.none)
        {
            return;
        }
        
        Enemy enemy;
        //instantiate
        switch (enemyType)
        {
            default:
            case(EnemyType.basic):
            {
                GameObject newEnemy = Instantiate(basicEnemyPrefab, spawnPoints[spawnPoint].position, Quaternion.identity, transform);
                enemy = newEnemy.GetComponent<Enemy>();
                break;
            }
            case(EnemyType.shield):
            {
                GameObject newEnemy = Instantiate(shieldEnemyPrefab, spawnPoints[spawnPoint].position, Quaternion.identity, transform);
                enemy = newEnemy.GetComponent<Enemy>();
                break;
            }
        }
        enemy.transform.LookAt(GameManager.Instance.Player.transform.position);
        aliveEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);
        GameManager.Instance.AddScore(1);
    }

    // public void GetSpawnCount(List<Transform> enemies)
    // {
    //     GameManager.Instance.SpawnCount = spawnPoints.Count;
    // }
}

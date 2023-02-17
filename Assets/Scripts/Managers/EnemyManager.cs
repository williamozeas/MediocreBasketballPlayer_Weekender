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
    
    private void Awake()
    {
        GameManager.Instance.enemyManager = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameManager.WaveStart += SpawnWave;
    }
    
    private void OnDisable()
    {
        GameManager.WaveStart -= SpawnWave;
    }

    IEnumerator EndRound()
    {
        yield return new WaitForSeconds(0.5f);
        
        //temp: end game
        GameManager.Instance.GameState = GameState.GameEnd;

        //TODO:
        //add UI or FX?
        //give health back?
        // GameManager.Instance.StartNextWave();
    }

    void SpawnWave(Wave wave)
    {
        StartCoroutine(SpawnWaveCoroutine(wave));
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
        //wait for all enemies to be dead
        while (aliveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(EndRound());
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
}

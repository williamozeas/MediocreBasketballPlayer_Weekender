using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//may be overdesigning but.....

public enum EnemyType
{
    none, //used in enemy spawning
    basic,
    shield
}

[System.Serializable]
public class EnemySet //Set of enemies that spawn at the same time
{
    public EnemyType[] enemies = new EnemyType[8]
    {
        EnemyType.none, EnemyType.none, EnemyType.none, EnemyType.none, 
        EnemyType.none, EnemyType.none, EnemyType.none, EnemyType.none,
    };
    //Do we wait for all enemies to be dead before spawning in next set
    public bool waitForAllEnemiesDead = false;

    public int GetEnemyTotal()
    {
        int total = 0;
        foreach (EnemyType type in enemies)
        {
            if (type != EnemyType.none)
            {
                total++;
            }
        }
        return total;
    }
}

[System.Serializable]
public class Wave
{
    public float timeBetweenSets = 4f;
    public List<EnemySet> enemySets;

    public int GetEnemyTotal()
    {
        int total = 0;
        foreach (EnemySet set in enemySets)
        {
            total += set.GetEnemyTotal();
        }

        return total;
    }
}

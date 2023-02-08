using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int health = 5;
    public int damage = 5;

    public EnemyStats(int health_, int damage_)
    {
        health = health_;
        damage = damage_;
    }
}

//basic enemy class for other enemies
public class Enemy : MonoBehaviour
{
    [Header("Stats")] public EnemyStats stats;
    public EnemyType Type => EnemyType.basic; //this is a property, meaning we can override it in a child class
    
    [Header("Current Stats")]
    public int health = 5;

    private Coroutine movingCoroutine;
    
    // Start is called on spawn
    void Start()
    {
        movingCoroutine = StartCoroutine(MoveTowardsPlayer());
        health = stats.health;
    }

    protected virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    //TODO: be called when enemy hits the player
    protected virtual IEnumerator Attack()
    {
        GameManager.Instance.Player.TakeDamage(stats.damage);
        StopCoroutine(movingCoroutine);
        yield return null;
        
        //TODO: death/attack animation, particle effects, etc.
        
        GameManager.Instance.enemyManager.RemoveEnemy(this);
        Destroy(gameObject);
    }

    protected virtual IEnumerator Die()
    {
        GameManager.Instance.enemyManager.RemoveEnemy(this);
        StopCoroutine(movingCoroutine);
        yield return null;
        
        //TODO: death animation, particle effects, etc.
        
        Destroy(gameObject);
    }

    protected virtual IEnumerator MoveTowardsPlayer()
    {
        //loop is infinite until these can die
        // while (GameManager.Instance.GameState = GameState.Playing)
        // {
        //     yield return null;
        // }
        yield return null;
    }
}

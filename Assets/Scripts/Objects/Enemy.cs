using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public int health = 5;
    public int damage = 5;
    public float speed = 0.5f;

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
    ParticleSystem explosion;
    private Rigidbody rigidbody;
    
    // Start is called on spawn
    void Start()
    {
        movingCoroutine = StartCoroutine(MoveTowardsPlayer());
        health = stats.health;
        explosion = GetComponent<ParticleSystem>();
        rigidbody = GetComponent<Rigidbody>();
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
        //Particle
        GetComponent<MeshRenderer>().enabled = false;
        explosion.Play();
        yield return new WaitForSeconds(0.3f);
        
        Destroy(gameObject);
    }

    protected virtual IEnumerator MoveTowardsPlayer()
    {
        if (!rigidbody)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        while (GameManager.Instance.GameState == GameState.Playing && health > 0)
        {
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            transform.LookAt(playerPos, Vector3.up);
            rigidbody.velocity = transform.forward * stats.speed;
            yield return null;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Basketballs"))
        //Hit a ball
        {
            TakeDamage(GameManager.Instance.Player.damage);
        } else if (other.gameObject.CompareTag("MainCamera"))
        {
            StartCoroutine(Attack());
            //TODO: sfx, etc?
            Destroy(gameObject);
        }
    }
}

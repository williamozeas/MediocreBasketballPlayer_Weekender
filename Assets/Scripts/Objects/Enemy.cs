using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

    private bool attacking = false;

    private Coroutine movingCoroutine;
    ParticleSystem explosion;
    private Rigidbody rigidbody;
    private NavMeshAgent agent;

    private FMOD.Studio.EventInstance growl;
    private Coroutine audioCoroutine;

    private void Awake()
    {
        explosion = GetComponent<ParticleSystem>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called on spawn
    void Start()
    {
        audioCoroutine = StartCoroutine(GrowlCoroutine());
        
        health = stats.health;
        movingCoroutine = StartCoroutine(MoveTowardsPlayer());
    }

    private void OnEnable()
    {
        GameManager.GameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameManager.GameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        StopCoroutine(movingCoroutine);
        agent.isStopped = true;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    //TODO: be called when enemy hits the player
    public virtual IEnumerator Attack()
    {
        if (!attacking)
        {
            //SFX
            growl.stop(STOP_MODE.ALLOWFADEOUT);
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Attack", gameObject);
            
            attacking = true;
            GameManager.Instance.Player.TakeDamage(stats.damage);
            GameManager.Instance.enemyManager.RemoveEnemy(this);
            StopCoroutine(movingCoroutine);
            yield return null;

            //TODO: death/attack animation, particle effects, etc.

            Destroy(gameObject);
        }
    }

    protected virtual IEnumerator Die()
    {
        //SFX
        StopCoroutine(audioCoroutine);
        growl.stop(STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Death", gameObject);
        
        GameManager.Instance.enemyManager.RemoveEnemy(this);
        var colliders = GetComponentsInChildren<Collider>();
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
        StopCoroutine(movingCoroutine);
        yield return null;
        
        //TODO: death animation, particle effects, etc.
        //Particle
        var meshes = GetComponentsInChildren<MeshRenderer>();
        foreach(var mesh in meshes)
        {
            mesh.enabled = false;
        }
        
        explosion.Play();
        yield return new WaitForSeconds(0.5f);
        
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
            // Vector3 playerPos = GameManager.Instance.Player.transform.position;
            // transform.LookAt(playerPos, Vector3.up);
            // rigidbody.velocity = transform.forward * stats.speed;
            // yield return null;
            agent.SetDestination(GameManager.Instance.Player.transform.position);
            
            yield return null;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Basketballs"))
        //Hit a ball
        {
            if (other.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 6f) {
                TakeDamage(GameManager.Instance.Player.damage);
            }
        } else if (other.gameObject.CompareTag("Player"))
        {
            if (!GameManager.Instance.Player.invincible)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator GrowlCoroutine()
    {
        while (true)
        {
            if (Random.Range(0f, 1f) > 0.8)
            {
                
                growl = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Enemies/Growl");
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(growl, transform);
                growl.start();
                growl.release();
                //Debug.Log("Playing!");
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.8f + Random.Range(0, 1));
        }
    }
}

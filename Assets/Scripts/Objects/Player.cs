using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    
    private void Awake()
    {
        GameManager.Instance.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        
        //TODO: animation, sound, etc

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.GameState = GameState.GameEnd;
    }
}

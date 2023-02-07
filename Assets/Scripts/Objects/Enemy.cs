using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic enemy class for other enemies
public class Enemy : MonoBehaviour
{
    //this is a property, meaning we can override it in a child class
    public EnemyType Type => EnemyType.basic;
    
    // Start is called on spawn
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic enemy class for other enemies
public class Enemy : MonoBehaviour
{
    ParticleSystem explosion;

    // Start is called before the first frame update
    void Start()
    {
        explosion = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        //Hit a ball
        {
            //Particle
            StartCoroutine(Die());   
        }
    }

    IEnumerator Die()
    {
        GetComponent<MeshRenderer>().enabled = false;
        explosion.Play();
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}

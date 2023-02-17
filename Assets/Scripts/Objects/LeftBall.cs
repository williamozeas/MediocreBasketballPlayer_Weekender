using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBall : MonoBehaviour
{
    int ballDestroyTime = 5;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, ballDestroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 1f);
    }
}

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
        Debug.Log(collision.impulse.magnitude);
        FMOD.Studio.EventInstance bounce = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Ball Bounce");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(bounce, transform);
        bounce.setParameterByName("NormalSpeed", collision.impulse.magnitude);
        bounce.start();
        bounce.release();
        Destroy(gameObject, 1.2f);
    }
}

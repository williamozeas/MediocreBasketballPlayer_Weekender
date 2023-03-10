using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBall : MonoBehaviour
{

    Rigidbody rb;

    float lifeTime;

    bool collided;

    public GameObject explosion;

    public float expTime = 0.1f;

    int ballDestroyTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lifeTime = 0f;
        collided = false;
        Destroy(gameObject, ballDestroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collided) {
            collided = true;
            
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Player/Lob", gameObject);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/VO/RandomVO");
            CameraShake.i.Shake(2);
            GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            ParticleSystem p =  exp.GetComponent<ParticleSystem>();
            var main = p.main;
            main.startSpeed = lifeTime * 3f / expTime;
            main.startLifetime = expTime;
            p.Play();
            Destroy(gameObject, .1f);
            ExplosionDamage(transform.position, lifeTime * 3f);
        }
    }

    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null) rb.AddExplosionForce(250f, transform.position, radius, 3.0f);

            if (hitCollider){
                hitCollider.SendMessage("TakeDamage", GameManager.Instance.Player.damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

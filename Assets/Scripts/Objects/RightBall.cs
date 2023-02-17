using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBall : MonoBehaviour
{

    Rigidbody rb;

    float lifeTime;

    bool collided;

    public GameObject explosion;

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
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject, .05f);
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

            //if (hitCollider.tag == "Shielded" || hitCollider.tag == "Base"){
                hitCollider.SendMessage("TakeDamage", GameManager.Instance.Player.damage);
            //}
        }
    }
}

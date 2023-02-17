using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Enemy owner;
    private Vector3 restPos;
    private Quaternion restRot;

    private float speed = 8f;
    private float rotationSpeed = 1f;

    private int maxHitCount = 5;
    private int hitCount;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        hitCount = 0;

        restPos = transform.localPosition;
        restRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector3.zero) {
            rb.velocity *= .98f;
        }
        if (rb.angularVelocity != Vector3.zero) {
            rb.angularVelocity *= .98f;
        }
        //if (Vector3.Distance(transform.localPosition, restPos) > 0.01f) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, restPos, speed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, restRot, rotationSpeed * Time.deltaTime);
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("colliding");
        if (collision.collider.gameObject.layer == 6)
        //Hit a ball
        {
            hitCount++;
            if (hitCount == maxHitCount) {
                Destroy(gameObject);
            }
        } else if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(owner.Attack());
        }
    }
}

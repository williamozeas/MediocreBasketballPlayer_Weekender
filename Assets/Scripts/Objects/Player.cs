using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    

    /*private float miny = -60f;
    private float maxy = -60f;
    //side to side turn
    private float minx = -60f;
    private float maxx = -60f;*/
    //up and down turn

    private float yRotation = 0f;
    private float xRotation = 0f;
    
    private float sensitivity = 400f;

    public GameObject ballPrefab;

    private float normalDistance = 20f;

    private int ballDestroyTime = 5;

    private int shootMode;

    private void Awake()
    {
        GameManager.Instance.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        shootMode = 1;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        //THese are diff than X and Y positions

        yRotation += mouseX * sensitivity * Time.deltaTime;
        xRotation += mouseY * sensitivity * Time.deltaTime;


        Quaternion localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        transform.rotation = localRotation;

        if (Input.GetKeyDown("1")) {
            //direct shot
            shootMode = 1;
        }
        if (Input.GetKeyDown("2")) {
            //arcing shot
            shootMode = 2;
        }

        if (Input.GetMouseButtonDown(0)) {

            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
            Rigidbody ballRB = ball.GetComponent<Rigidbody>();

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            Vector3 normalizedSideToSide = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

            if (shootMode == 1) {
                // Cast a ray out.
                if (Physics.Raycast(ray, out hit))
                {
                    //Add v_x
                    ballRB.velocity = normalizedSideToSide * normalDistance * (float)(Math.Cos(transform.forward.y));
                    //Add v_y
                    ballRB.velocity += Vector3.up * normalDistance * ((float)(Math.Sin(transform.forward.y)) + (4.9f * hit.distance / (normalDistance * normalDistance)));
                } else {
                    //Add v_x
                    ballRB.velocity = normalizedSideToSide * normalDistance;
                    //Add v_y
                    ballRB.velocity += Vector3.up * (normalDistance * (float)(Math.Tan(transform.forward.y)) + 4.9f);
                }
            } else if (shootMode == 2) {
                if (Physics.Raycast(ray, out hit))
                {
                    float effDist = hit.distance + 1f;
                    
                    double hitTime = Math.Sqrt(effDist * (1.73 * Math.Cos(transform.forward.y) - Math.Sin(transform.forward.y)) / 4.9);

                    float velX = effDist * (float)(Math.Cos(transform.forward.y)) / (float)(hitTime);

                    ballRB.velocity = normalizedSideToSide * velX;

                    ballRB.velocity += Vector3.up * velX * 1.73f;

                } else {
                    float vel = 10f;

                    ballRB.velocity = normalizedSideToSide * vel;

                    ballRB.velocity += Vector3.up * vel * 1.73f;
                }
            }

            

            Destroy(ball, ballDestroyTime);
            //ballRB.velocity = ball.transform.forward * 10f + ball.transform.up * 10f;
        }
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    public int damage = 5;
    

    private float yMin = -40f;
    private float yMax = 40f;
    //side to side turn
    private float xMin = -40f;
    private float xMax = 40f;
    //up and down turn

    private float yRotation = 0f;
    private float xRotation = 0f;

    public float chargeRate = 20f;
    public float minChargedVelocity = 2f;
    private float chargedVelocity = 0f;
    
    private float sensitivity = 600f;

    public GameObject ballPrefab;

    private float normalDistance = 20f;

    private int ballDestroyTime = 5;

    private int shootMode;

    private GameObject currentBall;
    private Rigidbody ballRB;
    private Vector3 normalizedSideToSide;
    

    private void Awake()
    {
        GameManager.Instance.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        shootMode = 2;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        chargedVelocity = minChargedVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        //THese are diff than X and Y positions

        yRotation += mouseX * sensitivity * Time.deltaTime;
        xRotation += mouseY * sensitivity * Time.deltaTime;

        yRotation = Math.Clamp(yRotation, yMin, yMax);
        xRotation = Math.Clamp(xRotation, xMin, xMax);


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

            currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
            ballRB = currentBall.GetComponent<Rigidbody>();

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            

            normalizedSideToSide = transform.forward.normalized;
            // Debug.DrawRay(transform.position, normalizedSideToSide, Color.red, 5);

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
                Destroy(currentBall, ballDestroyTime);
            } else if (shootMode == 2) {
                // if (Physics.Raycast(ray, out hit))
                // {
                //     float effDist = hit.distance + 1f;
                //     
                //     double hitTime = Math.Sqrt(effDist * (1.73 * Math.Cos(transform.forward.y) - Math.Sin(transform.forward.y)) / 4.9);
                //
                //     float velX = effDist * (float)(Math.Cos(transform.forward.y)) / (float)(hitTime);
                //
                //     ballRB.velocity = normalizedSideToSide * velX;
                //
                //     ballRB.velocity += Vector3.up * velX * 1.73f;
                //
                // } else {
                //     float vel = 10f;
                //
                //     ballRB.velocity = normalizedSideToSide * vel;
                //
                //     ballRB.velocity += Vector3.up * vel * 1.73f;
                // }
                ballRB.useGravity = false;
            }
            //ballRB.velocity = ball.transform.forward * 10f + ball.transform.up * 10f;
        }

        if (Input.GetMouseButton(0) && shootMode == 2) //charging
        {
            chargedVelocity += chargeRate * Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && shootMode == 2) //mouse up, shoot
        {
            ballRB.useGravity = true;
            Debug.Log(chargedVelocity);
            ballRB.velocity = normalizedSideToSide * chargedVelocity;
            ballRB.velocity += chargedVelocity * 1.73f * Vector3.up;
            Destroy(currentBall, ballDestroyTime);
            chargedVelocity = minChargedVelocity;
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

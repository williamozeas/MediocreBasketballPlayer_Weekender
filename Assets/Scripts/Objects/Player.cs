using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;
    public int damage = 5;
    public float cooldown = 0.5f;

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
    private float timeSinceThrow = 0;
    private bool throwing = false;
    
    public LineRenderer line;
    private int lineNumber = 20;

    private IEnumerator deleting;

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
        line.positionCount = lineNumber;
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

        timeSinceThrow += Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) {

            /*currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
            ballRB = currentBall.GetComponent<Rigidbody>();

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);*/
            

            
            // Debug.DrawRay(transform.position, normalizedSideToSide, Color.red, 5);

            /*if (shootMode == 1) {
                currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
                ballRB = currentBall.GetComponent<Rigidbody>();
                normalizedSideToSide = transform.forward.normalized;
                
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
            } else if (shootMode == 2 && timeSinceThrow > cooldown) {
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
                //currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
                //ballRB = currentBall.GetComponent<Rigidbody>();
                normalizedSideToSide = transform.forward.normalized;
                
                //ballRB.useGravity = false;
                throwing = true;
            }
            //ballRB.velocity = ball.transform.forward * 10f + ball.transform.up * 10f;*/
        }

        if (Input.GetMouseButtonDown(0) && shootMode == 2 && timeSinceThrow > cooldown) {
            
            if (deleting != null) StopCoroutine(deleting);
            line.startColor = new Color(1f, .5608f, .0118f, .8f);
            line.endColor = new Color(1f, .2823f, 0f, .8f);
            throwing = true;
        }

        if (Input.GetMouseButton(0) && throwing && shootMode == 2) //charging
        {
            chargedVelocity += chargeRate * Time.deltaTime;
            normalizedSideToSide = transform.forward.normalized;

            Vector3 prelimVel = normalizedSideToSide * chargedVelocity;
            prelimVel += chargedVelocity * 1.73f * Vector3.up;

            DrawShootingLine(prelimVel);
        }

        if (Input.GetMouseButtonUp(0) && throwing && shootMode == 2) //mouse up, shoot
        {
            deleting = EraseShootingLine();
            StartCoroutine(deleting);
            currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
            ballRB = currentBall.GetComponent<Rigidbody>();
            ballRB.useGravity = true;
            ballRB.velocity = normalizedSideToSide * chargedVelocity;
            ballRB.velocity += chargedVelocity * 1.73f * Vector3.up;
            Destroy(currentBall, ballDestroyTime);
            chargedVelocity = minChargedVelocity;
            timeSinceThrow = 0;
            throwing = false;
        }
    }

    IEnumerator EraseShootingLine () {
        for (float i = .8f; i >= 0f; i -= .02f) {
            line.startColor = new Color(1f, .5608f, .0118f, i);
            line.endColor = new Color(1f, .2823f, 0f, i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void DrawShootingLine (Vector3 vel) {

        float timeToDraw = (vel.y + (float)Math.Sqrt(vel.y * vel.y + 39.2f)) / 9.8f;

        Debug.Log("drawing");

        for (int i = 0; i < lineNumber; i++) {
            line.SetPosition(i, CalculatePos(vel, timeToDraw * i / lineNumber));
            //line.SetPosition(i, new Vector3(0, 0, i));
        }
       
    }

    private Vector3 CalculatePos (Vector3 vel, float t) {

        Vector3 vel_h = new Vector3(vel.x, 0f, vel.z);

        Vector3 pos = transform.position;
        pos += vel_h * t;
        pos.y += vel.y * t - 4.9f * t * t;

        return pos;
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

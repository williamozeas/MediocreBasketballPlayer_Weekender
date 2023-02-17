using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int health;
    public int damage;

    public float cooldown;
    bool canShoot;
    bool shooting;

    public float chargeRate;
    public float minChargedVelocity;
    public float maxChargedVelocity;
    [HideInInspector] public float chargedVelocity = 0f;
    [HideInInspector] public float percentCharge = 0f;

    public GameObject leftBallPrefab;
    public GameObject rightBallPrefab;

    int ballDestroyTime = 5;

    GameObject currentBall;
    Rigidbody ballRB;
    bool throwing = false;
    public bool IsThrowing => throwing;
    
    public LineRenderer line;
    int lineVertices = 21;

    IEnumerator deleting;

    public GameObject handBall;
    public Transform ballPos;
    /*private Vector3 handBallPos = new Vector3 (0.0285f, -0.0805f, 0.0235f);
    private Vector3 handBallShotPos = new Vector3 (0.1285f, -0.1805f, 0.1235f);
    private Vector3 handDir = new Vector3 (1f, -1f, -1f);
    private float handMoveSpeed = .001f;
    private Vector3 handLastPos;*/

    //float leftTime;

    RaycastHit dirHit;
    public float shotPowerMult;

    public Transform camTrans;

    Rigidbody rb;

    float gravity = 20f;

    public float rightLobAngle;
    float rightLobAngleDeg;
    float rightLobAngleTan;
    Vector3 finalDir;

    private void Awake()
    {
        GameManager.Instance.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        chargedVelocity = minChargedVelocity;
        line.positionCount = lineVertices;
        health = maxHealth;
        //handBallPos = handBall.transform.localPosition;
        for (int i = 0; i < lineVertices; i++) {
            line.SetPosition(i, new Vector3(0f, -10f, -10f));
        }
        canShoot = true;
        rb = GetComponent<Rigidbody>();
        rightLobAngleDeg = rightLobAngle * Mathf.PI / 180;
        rightLobAngleTan = MathF.Tan(rightLobAngleDeg);
    }

    private void OnEnable()
    {
        GameManager.GameStart += OnGameStart;
        GameManager.GameOver += OnGameOver;
        GameManager.GoToMenu += OnGoToMenu;
    }

    private void OnDisable()
    {
        GameManager.GameStart -= OnGameStart;
        GameManager.GameOver -= OnGameOver;
    }

    private void OnGoToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void OnGameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && !shooting) {
            if (CheckIfCanDunk())
            {
                Dunk();
                return;
            }
            canShoot = false;
            shooting = true;
        }
        if (Input.GetMouseButtonUp(0) && shooting) {
            LeftClickShoot();
            shooting = false;
            StartCoroutine(CoolDown());
        }

        if (Input.GetMouseButtonDown(1) && canShoot && !shooting) {
            canShoot = false;
            shooting = true;
            if (deleting != null) StopCoroutine(deleting);
            line.startColor = new Color(1f, .5608f, .0118f, .8f);
            line.endColor = new Color(1f, .2823f, 0f, .8f);
        }

        if (Input.GetMouseButton(1) && shooting)
        {
            RightHoldCharge();
        }

        if (Input.GetMouseButtonUp(1) && shooting) //mouse up, shoot
        {
            RightHoldShoot();
            shooting = false;
            StartCoroutine(CoolDown());
        }
    }

    private void LeftClickShoot()
    {
        //call handball to do an anim type deal

        Vector3 shootDirection = camTrans.forward;

        if(Physics.Raycast(transform.position, camTrans.forward, out dirHit)) {
             shootDirection = (dirHit.point - ballPos.position).normalized;
        }

        Vector3 shootDirectionHoriz = Vector3.Scale(shootDirection, new Vector3(1f, 0f, 1f)).normalized;
        Vector3 shootDirectionRight = Vector3.Cross(shootDirectionHoriz, Vector3.up).normalized;
        
        Vector3 finalDir = Quaternion.Euler(shootDirectionRight * 20) * shootDirection;

        // Vector3 finalDir = shootDirection;

        currentBall = Instantiate(leftBallPrefab, ballPos.position, Quaternion.identity) as GameObject;
        ballRB = currentBall.GetComponent<Rigidbody>();
        ballRB.useGravity = true;
        // ballRB.velocity = new Vector3(rb.velocity.x/2, rb.velocity.y/4, rb.velocity.z/2);
        ballRB.velocity = 0.5f * Math.Clamp(Vector3.Dot(finalDir, rb.velocity), 0, 100) * finalDir;
        ballRB.velocity = new Vector3(ballRB.velocity.x, 0f, ballRB.velocity.z);
        ballRB.AddForce(rb.velocity, ForceMode.Impulse);
        ballRB.AddForce(finalDir * shotPowerMult, ForceMode.Impulse);
        Destroy(currentBall, ballDestroyTime);
    }

    private void RightHoldCharge()
    {
        if(chargedVelocity < maxChargedVelocity) {
            chargedVelocity += chargeRate * Time.deltaTime;
        }

        Vector3 shootDirection = camTrans.forward;
        Vector3 shootDirectionHoriz = Vector3.Scale(shootDirection, new Vector3(1f, 0f, 1f)).normalized;

        finalDir = shootDirectionHoriz * chargedVelocity;
        finalDir += chargedVelocity * rightLobAngleTan * Vector3.up;

        DrawShootingLine(finalDir);
    }

    private void RightHoldShoot()
    {
        deleting = EraseShootingLine();
        StartCoroutine(deleting);

        currentBall = Instantiate(rightBallPrefab, ballPos.position, Quaternion.identity) as GameObject;
        ballRB = currentBall.GetComponent<Rigidbody>();
        ballRB.useGravity = true;
        ballRB.velocity = finalDir;
        Destroy(currentBall, ballDestroyTime);
        chargedVelocity = minChargedVelocity;
    }

    IEnumerator EraseShootingLine()
    {
        for (float i = .8f; i >= 0f; i -= .02f) {
            line.startColor = new Color(1f, .5608f, .0118f, i);
            line.endColor = new Color(1f, .2823f, 0f, i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void DrawShootingLine (Vector3 vel)
    {
        float timeToDraw = (vel.y + (float)Math.Sqrt(vel.y * vel.y + 4.0f * gravity)) / gravity;

        for (int i = 0; i < lineVertices; i++) {
            line.SetPosition(i, CalculatePos(vel, timeToDraw * (float)i / (float)(lineVertices - 1)));
        }
       
    }

    private Vector3 CalculatePos (Vector3 vel, float t)
    {
        Vector3 vel_h = new Vector3(vel.x, 0f, vel.z);

        Vector3 pos = ballPos.position;
        pos += vel_h * t;
        pos.y += vel.y * t - (gravity / 2.0f) * t * t;

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

    private bool CheckIfCanDunk()
    {
        return false; //unimplemented
    }

    private void Dunk()
    {
        return; //unimplemented
    }

    public void Die()
    {
        GameManager.Instance.GameState = GameState.GameEnd;
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}







/*if (Input.GetMouseButtonDown(0)) {
    //direct shot
    shootMode = 1;
}
if (Input.GetMouseButtonDown(1)) {
    //arcing shot
    shootMode = 2;
}

timeSinceThrow += Time.deltaTime;

if (Input.GetMouseButtonDown(0) && timeSinceThrow > cooldown || Input.GetMouseButtonDown(1) && timeSinceThrow > cooldown) {
    if (deleting != null) StopCoroutine(deleting);
    line.startColor = new Color(1f, .5608f, .0118f, .8f);
    line.endColor = new Color(1f, .2823f, 0f, .8f);
    throwing = true;
}

if (Input.GetMouseButton(1) && throwing && shootMode == 2) //charging
{
    if(chargedVelocity < maxChargedVelocity) {
        chargedVelocity += chargeRate * Time.deltaTime;
    }
    normalizedSideToSide = transform.forward.normalized;

    Vector3 prelimVel = normalizedSideToSide * chargedVelocity;
    prelimVel += chargedVelocity * 1.73f * Vector3.up;

    if (handBall.transform.localPosition.x < handBallShotPos.x) handBall.transform.localPosition += handDir * handMoveSpeed;

    DrawShootingLine(prelimVel);
} else if (Input.GetMouseButton(0) && throwing && shootMode == 1) {
    if(chargedVelocity < maxChargedVelocity * 3) {
        chargedVelocity += chargeRate * Time.deltaTime * 3;
    }
    normalizedSideToSide = transform.forward.normalized;

    Vector3 prelimVel = normalizedSideToSide * chargedVelocity;
    prelimVel += chargedVelocity * .36f * Vector3.up;

    if (handBall.transform.localPosition.x < handBallShotPos.x) handBall.transform.localPosition += handDir * handMoveSpeed;

    DrawShootingLine(prelimVel);
}

if (Input.GetMouseButtonUp(1) && throwing && shootMode == 2) //mouse up, shoot
{
    handLastPos = handBall.transform.localPosition;
    deleting = EraseShootingLine(2);
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
} else if (Input.GetMouseButtonUp(0) && throwing && shootMode == 1) //mouse up, shoot
{
    handLastPos = handBall.transform.localPosition;
    deleting = EraseShootingLine(1);
    StartCoroutine(deleting);
    currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity) as GameObject;
    ballRB = currentBall.GetComponent<Rigidbody>();
    ballRB.useGravity = true;
    ballRB.velocity = normalizedSideToSide * chargedVelocity;
    ballRB.velocity += chargedVelocity * .36f * Vector3.up;
    Destroy(currentBall, ballDestroyTime);
    chargedVelocity = minChargedVelocity;
    timeSinceThrow = 0;
    throwing = false;
}

if (!throwing) {
    float time = timeSinceThrow / cooldown;
    float timeScale = Mathf.Clamp01(time);
    handBall.transform.localPosition = Vector3.Slerp(handLastPos, handBallPos, timeScale);
}

if (shootMode == 1) {
    percentCharge = chargedVelocity / (maxChargedVelocity * 3);
} else if (shootMode == 2) {
    percentCharge = chargedVelocity / (maxChargedVelocity);
}*/
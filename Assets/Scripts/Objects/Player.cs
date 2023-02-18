using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int health;
    public int damage;
    [HideInInspector] public bool invincible = false;

    public float cooldown;
    bool canShoot;
    bool shooting;
    
    [Header("Charging Variables")]
    public float chargeRate;
    public float minChargedVelocity;
    public float maxChargedVelocity;
    [HideInInspector] public float chargedVelocity = 0f;
    [HideInInspector] public float percentCharge = 0f;

    [Header("Dunking Variables")] 
    public float dunkingMinHeight = 1f;
    public float dunkingMaxAngle = 50f; //degrees
    public float dunkingMaxDistance = 3f;
    public float dunkingVelocity = 3f;
    public float dunkingInvincibilityTime = 0.4f;
    public float dunkingExplosionPower = 500f;
    public float dunkingExplosionRadius = 2f;
    
    [Header("References")]
    public GameObject leftBallPrefab;
    public GameObject rightBallPrefab;
    
    //vignette stuff
    private Vignette damageVignette;
    private Coroutine damageAnim;

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
    private Move move;

    float gravity = 20f;

    public float rightLobAngle;
    float rightLobAngleDeg;
    float rightLobAngleTan;
    Vector3 finalDir;

    private void Awake()
    {
        GameManager.Instance.SetPlayer(this);
        rb = GetComponent<Rigidbody>();
        move = GetComponent<Move>();
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
        rightLobAngleDeg = rightLobAngle * Mathf.PI / 180;
        rightLobAngleTan = MathF.Tan(rightLobAngleDeg);
        if (!GameManager.Instance.Volume.profile.TryGet<Vignette>(out damageVignette))
        {
            Debug.LogWarning("No Vignette found!");
        }
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
        GameManager.GoToMenu -= OnGoToMenu;
    }

    private void OnGoToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        health = maxHealth;
        transform.position = new Vector3(0, 1, 0);
        transform.LookAt(new Vector3(0, 1, 1));
        damageVignette.intensity.value = 0f;
    }
    
    private void OnGameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        invincible = false;
    }

    private void OnGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        rb.velocity = Vector3.zero;
        invincible = true;
    }

    void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0) && canShoot && !shooting)
            {
                Enemy theDunkedOne = CheckIfCanDunk();
                if (theDunkedOne != null)
                {
                    StartCoroutine(Dunk(theDunkedOne));
                    return;
                }
                else
                {
                    canShoot = false;
                    shooting = true;
                }
            }

            if (Input.GetMouseButtonUp(0) && shooting)
            {
                LeftClickShoot();
                shooting = false;
                StartCoroutine(CoolDown());
            }

            if (Input.GetMouseButtonDown(1) && canShoot && !shooting)
            {
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
    }

    private void LeftClickShoot()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Throw");
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
        //Destroy(currentBall, ballDestroyTime);
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Throw");
        deleting = EraseShootingLine();
        StartCoroutine(deleting);

        currentBall = Instantiate(rightBallPrefab, ballPos.position, Quaternion.identity) as GameObject;
        ballRB = currentBall.GetComponent<Rigidbody>();
        ballRB.useGravity = true;
        ballRB.velocity = finalDir;
        //Destroy(currentBall, ballDestroyTime);
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
        if (damageAnim != null)
        {
            StopCoroutine(damageAnim);
        }
        damageAnim = StartCoroutine(DamageAnimation());

        if (health <= 0)
        {
            Die();
        }
    }

    private Enemy CheckIfCanDunk()
    {
        if (move.Grounded)
        {
            return null;
        }
        LayerMask layerMask = LayerMask.GetMask("Enemies", "Ground", "Default");
        RaycastHit hit;
        
        //angle check
        float angleToUp = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.up, -camTrans.TransformDirection(Vector3.forward)));
        if (angleToUp > dunkingMaxAngle)
        {
            return null;
        }
        
        if (Physics.Raycast(camTrans.position, camTrans.TransformDirection(Vector3.forward), out hit, dunkingMaxDistance, layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemies") && hit.distance > dunkingMinHeight)
            {
                return hit.transform.GetComponent<Enemy>();
            }
            else
            {
                return null;
            }
        }
        return null; 
    }

    private IEnumerator Dunk(Enemy enemy)
    {
        float slowTime = 0.3f;
        float windUpTime = 0.3f;
        float enemyDistance = 0.15f;
        
        //disable colliders etc
        // var colliders = GetComponentsInChildren<Collider>();
        // foreach (Collider collider in colliders)
        // {
        //     collider.enabled = false;
        // }
        invincible = true;
        move.moveable = false;
        rb.useGravity = false;
        canShoot = false;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Shield"));
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/VO/Dunk");

        //Dunk!
        float drag = rb.drag;
        rb.drag = 0;
        Vector3 velocity = rb.velocity;
        for (float timeElapsed = 0f; timeElapsed < windUpTime; timeElapsed += Time.deltaTime)
        {
            //slowing to stop
            rb.velocity = velocity * EasingFunction.EaseOutQuart(1, 0, timeElapsed/slowTime);
            
            //winding up
            Vector3 directionVector = (enemy.transform.position - transform.position).normalized;
            rb.velocity += directionVector * EasingFunction.EaseInBack(0, dunkingVelocity, timeElapsed/windUpTime);
            // rb.velocity = directionVector * 100;
            if ((enemy.transform.position - transform.position).magnitude < enemyDistance)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        
        //wait for contact
        while ((enemy.transform.position - transform.position).magnitude < enemyDistance)
        {
            rb.velocity = (enemy.transform.position - transform.position).normalized * dunkingVelocity;
            yield return null;
        }
        
        //On Contact
        enemy.TakeDamage(enemy.health);
        rb.drag = drag;
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Lob");
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position - new Vector3(0, 0.4f, 0), dunkingExplosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null && hitCollider.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(dunkingExplosionPower, transform.position, dunkingExplosionRadius, 3.0f,
                    ForceMode.Impulse);
            }
        }
        yield return null;
        
        //reset
        move.moveable = true;
        rb.useGravity = true;
        canShoot = true;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Shield"), false);

        yield return new WaitForSeconds(dunkingInvincibilityTime);
        invincible = false;
        
        yield return new WaitForSeconds(0.8f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null && hitCollider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    rb.isKinematic = true;
                    rb.AddExplosionForce(dunkingExplosionPower, transform.position, dunkingExplosionRadius, 3.0f,
                        ForceMode.Impulse);
                }
            }
        }
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

    IEnumerator DamageAnimation()
    {
        float hold = 0.6f;
        float timeOut = 0.8f;
        float maxIntensity = 0.55f;
        float minIntensity = 0.3f;
        float lowHealth = (float)maxHealth / 5;
        float lowHealthIntensity = 0.25f;
        
        //start up
        float end = ((float)(maxHealth - health) / maxHealth) * (maxIntensity - minIntensity) + minIntensity;
        damageVignette.intensity.value = end;
        
        //hold
        yield return new WaitForSeconds(hold);

        //ease out
        float normalIntensity = 0;
        if (health < lowHealth)
        {
            normalIntensity = lowHealthIntensity;
        }
        Debug.Log(normalIntensity);
        for (float timeElapsed = 0f; timeElapsed < timeOut; timeElapsed += Time.deltaTime)
        {
            damageVignette.intensity.value = EasingFunction.EaseInOutQuad(end, normalIntensity, timeElapsed);
            yield return null;
        }
        damageVignette.intensity.value = normalIntensity;
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
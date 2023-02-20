using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Collider headCollider;
    public Collider mouthCollider;

    private Animator animator;
    public Animator outlineAnimator;

    public int maxHealth = 6;
    public int health = 6;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        GameManager.Instance.Boss = this;
    }

    private void OnEnable()
    {
        GameManager.WaveStart += OnWaveStart;
    }

    private void OnWaveStart(Wave wave)
    {
        health = maxHealth;
        animator.enabled = true;
        outlineAnimator.enabled = true;
        if (GameManager.Instance.Round == 3)
        {
            animator.SetBool("LastRound", true);
            outlineAnimator.SetBool("LastRound", true);
        }
        else
        {
            animator.SetBool("LastRound", false);
            outlineAnimator.SetBool("LastRound", false);
        }
    }

    public void OnLaugh()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/VO/Evil Laugh", gameObject);
        headCollider.enabled = false;
        mouthCollider.enabled = true;
    }

    public void OnIdle()
    {
        headCollider.enabled = true;
        mouthCollider.enabled = false;
    }

    public void TakeDamage()
    {
        health--;
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/VO/Ow", gameObject);
        if (health <= 0)
        {
            mouthCollider.enabled = false;
            StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        animator.enabled = false;
        outlineAnimator.enabled = false;
        GameManager.Instance.Player.invincible = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/VO/Wave 4");
        yield return new WaitForSeconds(2f);
        GameManager.Instance.GameState = GameState.GameEnd;
        GameManager.Instance.Player.invincible = false;
    }
}

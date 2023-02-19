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
        // if (GameManager.Instance.Round == 3)
        {
            animator.SetBool("LastRound", true);
            outlineAnimator.SetBool("LastRound", true);
        }
    }

    public void OnLaugh()
    {
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
        Debug.Log(health);
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
        yield return new WaitForSeconds(1f);
        GameManager.Instance.GameState = GameState.GameEnd;
    }
}

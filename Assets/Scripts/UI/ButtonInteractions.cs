using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractions : MonoBehaviour
{
    public GameObject MainBackground;
    public GameObject MainBackgroundLight;
    public GameObject LandingPage;
    public GameObject HowToPlayPage;
    public GameObject PlayPage;
    public GameObject EndingPage;

    private void OnEnable()
    {
        GameManager.GameOver += OnGameOver;
        GameManager.GoToMenu += OnGoToMenu;
    }

    private void OnDisable()
    {
        GameManager.GameOver -= OnGameOver;
        GameManager.GoToMenu -= OnGoToMenu;
    }

    public void OnGameOver()
    {
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(true);
        EndingPage.SetActive(true);
        PlayPage.SetActive(false);
    }
    
    private void OnGoToMenu()
    {
        LandingPage.gameObject.SetActive(true);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(false);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(true);
        MainBackgroundLight.SetActive(false);
    }

    public void clickStartFromMain() {
        LandingPage.gameObject.SetActive(false);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(true);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(false);

        GameManager.Instance.GameState = GameState.Playing;
    }

    public void clickHowToPlayFromMain() {
        LandingPage.gameObject.SetActive(false);
        HowToPlayPage.gameObject.SetActive(true);
        PlayPage.gameObject.SetActive(false);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(true);
        MainBackgroundLight.SetActive(false);
    }

    public void clickStartFromHowToPlay() {
        LandingPage.gameObject.SetActive(false);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(true);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(false);

        GameManager.Instance.GameState = GameState.Playing;
    }

    public void clickBackFromHowToPlay() {
        LandingPage.gameObject.SetActive(true);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(false);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(true);
        MainBackgroundLight.SetActive(false);
    }

    public void clickExitFromEnd() {
        GameManager.Instance.GameState = GameState.Menu;
        
        LandingPage.gameObject.SetActive(true);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(false);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(true);
        MainBackgroundLight.SetActive(false);
    }

    public void clickPlayAgainFromEnd() {
        GameManager.Instance.GameState = GameState.Menu;
        GameManager.Instance.GameState = GameState.Playing;
        
        LandingPage.gameObject.SetActive(false);
        HowToPlayPage.gameObject.SetActive(false);
        PlayPage.gameObject.SetActive(true);
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(false);
    }

    void Start()
    {
        MainBackground.gameObject.SetActive(true);
    }
}
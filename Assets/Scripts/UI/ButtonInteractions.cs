using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class ButtonInteractions : MonoBehaviour
{
    public GameObject MainBackground;
    public GameObject MainBackgroundLight;
    public GameObject LandingPage;
    public GameObject HowToPlayPage;
    public GameObject PlayPage;
    public GameObject EndingPage;
    public GameObject Intro, I1, I2, I3, I4, I5, I6, I7, I8, I9;

    public float time1, time2, time3, time4, time5, time6, time7, time8, time9;

    public bool introSeen;


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
        MainBackgroundLight.SetActive(false);
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
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(false);

        StartCoroutine(StartGame());
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
        EndingPage.gameObject.SetActive(false);
        MainBackground.SetActive(false);
        MainBackgroundLight.SetActive(false);

        StartCoroutine(StartGame());
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

        introSeen = false;
    }

    IEnumerator StartGame()
    {
        if (!introSeen){
            Intro.SetActive(true);
            I1.SetActive(true);
            yield return new WaitForSeconds(time1);
            I2.SetActive(true);
            Destroy(I1);
            yield return new WaitForSeconds(time2);
            I3.SetActive(true);
            Destroy(I2);
            yield return new WaitForSeconds(time3);
            I4.SetActive(true);
            Destroy(I3);
            yield return new WaitForSeconds(time4);
            I5.SetActive(true);
            Destroy(I4);
            yield return new WaitForSeconds(time5);
            I6.SetActive(true);
            Destroy(I5);
            yield return new WaitForSeconds(time6);
            I7.SetActive(true);
            Destroy(I6);
            yield return new WaitForSeconds(time7);
            I8.SetActive(true);
            Destroy(I7);
            yield return new WaitForSeconds(time8);
            I9.SetActive(true);
            Destroy(I8);
            yield return new WaitForSeconds(time9);
            Destroy(I9);
            Destroy(Intro);
            introSeen = true;
        }

        PlayPage.gameObject.SetActive(true);
        GameManager.Instance.GameState = GameState.Playing;
    }
}
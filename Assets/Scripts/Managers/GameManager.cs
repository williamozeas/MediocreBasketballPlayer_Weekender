using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = FMOD.Debug;

public enum GameState
{
    Playing,
    GameEnd,
    Pause,
    Menu
}

public class GameManager : Singleton<GameManager>
{
    private GameState _gamestate;
    public GameState GameState //GameState cannot be set without calling SetGameState
    {
        set { SetGameState(value); }
        get { return _gamestate; }
    }
    
    //Set in Awake() functions of player & enemyManager
    private Player player;
    public Player Player => player;
    public EnemyManager enemyManager;
    private int score; //idk if we want a score
    public int Score => score;
    
    //events - these can be recieved and trigger things all throughout the game
    public static event Action GameStart;
    public static event Action GameOver;
    public static event Action GoToMenu;

    // Start is called before the first frame update
    void Start()
    {
        SetGameState(GameState.Menu);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //Debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameState = GameState.Playing;
        }
#endif
    }

    public void SetGameState(GameState newGameState)
    {
        // Debug.Log("");
        switch (newGameState)
        {
            case (GameState.Menu):
            {
                GoToMenu?.Invoke();
                break;
            }
            case (GameState.Playing):
            {
                GameStart?.Invoke();
                break;
            }
            case (GameState.GameEnd):
            {
                GameOver?.Invoke();
                break;
            }
        }

        _gamestate = newGameState;
    }

    public void SetPlayer(Player playerIn)
    {
        player = playerIn;
    }

    public void AddScore(int addScore)
    {
        //in case we want UI VFX or something
        score += addScore;
    }
}

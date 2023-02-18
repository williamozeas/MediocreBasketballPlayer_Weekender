using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum GameState
{
    Playing,
    GameEnd,
    Pause,
    Menu
}

public class GameManager : Singleton<GameManager>
{
    [Header("Data")] public WaveData waveData;
    private GameState _gamestate;
    public GameState GameState //GameState cannot be set without calling SetGameState
    {
        set { SetGameState(value); }
        get { return _gamestate; }
    }
    
    //Set in Awake() functions of player & enemyManager
    private Player player;
    public Player Player => player;
    [HideInInspector] public EnemyManager enemyManager;
    private int score = 0; //number of enemies killed
    public int Score => score;
    private int round = 0;
    public int Round => round;
    private Volume volume;
    public Volume Volume => volume;
    
    //events - these can be recieved and trigger things all throughout the game
    public static event Action GameStart;
    public static event Action<Wave> WaveStart;
    public static event Action GameOver;
    public static event Action GoToMenu;

    public override void Awake()
    {
        base.Awake();
        volume = GameObject.Find("Global Volume").GetComponent<Volume>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetGameState(GameState.Menu);
    }

    public void SetGameState(GameState newGameState)
    {
        // Debug.Log("");
        switch (newGameState)
        {
            case (GameState.Menu):
            {
                GoToMenu?.Invoke();
                round = 0;
                score = 0;
                break;
            }
            case (GameState.Playing):
            {
                if (GameState == GameState.Menu || GameState == GameState.GameEnd)
                {
                    round = 0;
                    GameStart?.Invoke();
                    StartNextWave();
                }
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
    
    public void StartNextWave()
    {
        round++;
        WaveStart?.Invoke(waveData.waves[round]);
    }

    public void SetPlayer(Player playerIn)
    {
        player = playerIn;
    }

    public void AddScore(int addScore)
    {
        score += addScore;
    }
}

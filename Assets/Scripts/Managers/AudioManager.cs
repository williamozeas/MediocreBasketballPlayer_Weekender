using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class AudioManager : Singleton<AudioManager>
{
    private FMOD.Studio.EventInstance music;
    
    // Start is called before the first frame update
    void Start()
    {
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Music");
        music.start();
    }

    private void OnEnable()
    {
        GameManager.GameStart += OnGameStart;
        GameManager.GameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameManager.GameStart -= OnGameStart;
        GameManager.GameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        music.setParameterByName("Playing", 0);
    }

    private void OnGameStart()
    {
        music.setParameterByName("Playing", 1);
    }
    
    public static bool IsPlaying(FMOD.Studio.EventInstance instance) {
        FMOD.Studio.PLAYBACK_STATE state;   
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RoundHitData : MonoBehaviour
{
    private int waveTotal;
    public TextMeshProUGUI dataText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI bossText;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Round < 3)
        {
            waveTotal = GameManager.Instance.enemyManager.CurrentWaveTotal;
            dataText.text = (waveTotal - GameManager.Instance.Score).ToString() + "  /  " + waveTotal.ToString();
            roundText.text = "ROUND " + GameManager.Instance.Round.ToString();
        }
        else
        {
            waveTotal = GameManager.Instance.enemyManager.CurrentWaveTotal;
            bossText.enabled = true;
            dataText.enabled = false;
            roundText.text = "ROUND 3";
        }
    }
}

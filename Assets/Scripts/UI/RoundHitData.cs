using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundHitData : MonoBehaviour
{
    private int waveTotal;
    public TextMeshProUGUI dataText;
    public TextMeshProUGUI roundText;

    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        waveTotal = GameManager.Instance.enemyManager.CurrentWaveTotal;
        dataText.text = (waveTotal - GameManager.Instance.Score).ToString() + "  /  " + waveTotal.ToString();
        roundText.text = "ROUND " + GameManager.Instance.Round.ToString();
    }
}

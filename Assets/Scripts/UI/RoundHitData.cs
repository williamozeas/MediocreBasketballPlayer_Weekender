using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundHitData : MonoBehaviour
{
    public TextMeshProUGUI dataText;
    public TextMeshProUGUI roundText;

    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        dataText.text = GameManager.Instance.Score.ToString() + "  /  " + GameManager.Instance.SpawnCount.ToString();
        roundText.text = "ROUND " + GameManager.Instance.Round.ToString();
    }
}

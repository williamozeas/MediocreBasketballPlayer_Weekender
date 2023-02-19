using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinOrLose : MonoBehaviour
{
    public Image GameOverBg;
    public Image VictoryBg;
    public Image GameOverText;
    public Image VictoryText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameManager.Instance.GameState == GameState.GameEnd) && (GameManager.Instance.Player.health == 0))
        {
            GameOverBg.gameObject.SetActive(true);
            GameOverText.gameObject.SetActive(true);
            VictoryBg.gameObject.SetActive(false);
            VictoryText.gameObject.SetActive(false);
        }
        else
        {
            VictoryBg.gameObject.SetActive(true);
            VictoryText.gameObject.SetActive(true);
            GameOverBg.gameObject.SetActive(false);
            GameOverText.gameObject.SetActive(false);
        }
    }
}

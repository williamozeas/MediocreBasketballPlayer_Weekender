using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalRoundControl : MonoBehaviour
{
    public Slider BossHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Round != 3)
        {
            BossHealthBar.gameObject.SetActive(false);
        }
        else
        {
            BossHealthBar.gameObject.SetActive(true);
        }
    }

}

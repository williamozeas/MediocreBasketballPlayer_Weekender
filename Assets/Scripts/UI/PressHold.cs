using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressHold : MonoBehaviour
{
    public Slider slider;
    public Gradient slider_GradientColor;
    public Gradient slider_PlainColor;
    public Image slider_Fill;

    public float slider_StartingValue;
    public float slider_EndValue;
    public float slider_MinValue;

    public bool slider_BGFill;

    // Start is called before the first frame update
    void Start()
    {
        // slider_StartingValue = 0;
        slider_EndValue = 1f;
        slider_MinValue = 0f;
        slider.maxValue = slider_EndValue; // max power, to be set
        slider_Fill.color = slider_GradientColor.Evaluate(1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Player.IsThrowing) // The Button Space is in Hold State
        {
            slider_BGFill = true;
            slider_StartingValue = GameManager.Instance.Player.percentCharge;
        }
        else // The Button is Released or No Action Performed
        {
            slider_BGFill = false;
            slider_StartingValue = Mathf.MoveTowards(slider_StartingValue, slider_MinValue, 4f * Time.deltaTime);
        }

        slider.normalizedValue = slider_StartingValue/slider_EndValue;

        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     if (slider_BGFill == false) 
        //     {
        //         slider_BGFill = true;
        //     }
        //     else 
        //     {
        //         slider_BGFill = false;
        //     }
        // }

        if (slider_BGFill)
        {
            slider_Fill.color = slider_GradientColor.Evaluate(slider.normalizedValue);
        }
        else
        {
            slider_Fill.color = slider_PlainColor.Evaluate(slider.normalizedValue);
        }
    }
}

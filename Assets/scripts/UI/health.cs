using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class health : MonoBehaviour
{


    public GameObject player;
    public Slider HPSlider;
    public TMP_Text hpDisplay;
    public int totalHP;

    public static int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        HPSlider.minValue = 0;
        HPSlider.maxValue = totalHP;
        HPSlider.value = totalHP;
        currentHP = totalHP; 
    }

    // Update is called once per frame
    void Update()
    {
        hpDisplay.text = "Health: " + currentHP;
        HPSlider.value = currentHP;
    }

}


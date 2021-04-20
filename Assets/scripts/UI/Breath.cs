using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Breath : MonoBehaviour
{

    public Slider BreathSlider;
    public TMP_Text BreathText;
    public int breathRemaining;
    public int breathCapacity;

    // Start is called before the first frame update
    void Start()
    {
        BreathSlider.minValue = 0;
        BreathSlider.maxValue = breathCapacity;
        BreathSlider.value = breathCapacity;
        breathRemaining = breathCapacity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BreathText.text = "breath: " + (breathRemaining/100+1) + "s";
        StartCoroutine(breathing());
        StopCoroutine(breathing());
        BreathSlider.value = breathRemaining;
        if (breathRemaining <= 0)
        {
            breathRemaining = breathCapacity;
        }
    }

   IEnumerator breathing()
    {
        yield return new WaitForSeconds(1);
        breathRemaining--;
    }
}


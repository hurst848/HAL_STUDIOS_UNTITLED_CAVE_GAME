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

    public audioOutputController breathEmmision;
    public List<AudioClip> breathNoises;

    // Start is called before the first frame update
    void Start()
    {
        BreathSlider.minValue = 0;
        BreathSlider.maxValue = breathCapacity;
        BreathSlider.value = breathCapacity;
        breathRemaining = breathCapacity;
        StartCoroutine(breathNoiseController());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BreathText.text = "breath:";
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
        if (cameraController.isSprinting == true)
        {
            yield return new WaitForSeconds(0.5f);
            breathRemaining--;
        }
        else
        {
            yield return new WaitForSeconds(1);
            breathRemaining--;
        }
    }

    IEnumerator breathNoiseController()
    {
        yield return new WaitForSeconds(3.0f);

        while (health.currentHP != 0)
        {
            if(breathRemaining <= 0 && cameraController.isSprinting == true)
            {
                breathEmmision.sound = breathNoises[1];
                breathEmmision.triggerSound();
                yield return new WaitForSeconds(breathEmmision.gameObject.GetComponent<AudioSource>().clip.length);

                breathEmmision.sound = breathNoises[3];
                breathEmmision.triggerSound();
                yield return new WaitForSeconds(breathEmmision.gameObject.GetComponent<AudioSource>().clip.length);

            } else if (breathRemaining <= 0)
            {
                breathEmmision.sound = breathNoises[0];
                breathEmmision.triggerSound();
                yield return new WaitForSeconds(breathEmmision.gameObject.GetComponent<AudioSource>().clip.length);
            }
            yield return null;
        }
        yield return null;
    }
}


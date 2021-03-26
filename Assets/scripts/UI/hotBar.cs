using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hotBar : MonoBehaviour
{

    //private bool changeSprite;

    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public Image image5;

    public Sprite Knob;
    public Sprite checkMark;


    // Start is called before the first frame update
    void Start()
    {
        image1.sprite = Knob;
        image1.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        image2.sprite = Knob;
        image2.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        image3.sprite = Knob;
        image3.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        image4.sprite = Knob;
        image4.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        image5.sprite = Knob;
        image5.GetComponent<Image>().color = new Color(0, 0, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

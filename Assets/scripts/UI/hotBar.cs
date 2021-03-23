using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hotBar : MonoBehaviour
{

    private bool changeSprite;

    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public Image image5;

    public Sprite Knob;

    /*public void changeImage(string newImageTitle)
    {
        image1.sprite = Resources.Load<Sprite>("newImageTitle");
    }*/

    // Start is called before the first frame update
    void Start()
    {
        changeSprite = false;

        image1.GetComponent<Image>().color = new Color (0, 0, 0, 255);
        //image1.GetComponent<Image>().sprite = Knob;
        image2.GetComponent<Image>().color = new Color (0, 0, 0, 0);
        image3.GetComponent<Image>().color = new Color (0, 0, 0, 0);
        image4.GetComponent<Image>().color = new Color (0, 0, 0, 0);
        image5.GetComponent<Image>().color = new Color (0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeSprite = true;
        }
    }

    void itemStatus()
    {
        Sprite Knob = Resoucres.Load<Sprite>("Knob");

        if(changeSprite)
        {
            gameObject.GetComponent<Image>().sprite = Knob;
        }
    }
}

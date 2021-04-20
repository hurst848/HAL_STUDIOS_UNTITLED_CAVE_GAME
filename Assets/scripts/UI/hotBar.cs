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

    public Sprite None;
    public Sprite Knob;
    public Sprite checkMark;

    public static int pickUpType;
    public bool isImageFull1;
    public bool isImageFull2;
    public bool isImageFull3;
    public bool isImageFull4;
    public bool isImageFull5;

    // Start is called before the first frame update
    void Start()
    {

        image1.sprite = None;
        image1.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        isImageFull1 = false;

        image2.sprite = None;
        image2.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        isImageFull2 = false;

        image3.sprite = None;
        image3.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        isImageFull3 = false;

        image4.sprite = None;
        image4.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        isImageFull4 = false;

        image5.sprite = None;
        image5.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        isImageFull5 = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {

        }
    }

}

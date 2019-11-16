using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorPicker : MonoBehaviour{
    public Transform target;
    public Texture2D pallete;
    public Texture2D okBtn;
    public Color32 color;

    public GameObject PalleteImg;
    public Image colorRef;

    public bool isOn = false;

    int ImageWidth = 100;
    int ImageHeight = 100;

    Vector2 pickpos;
    public float offsetX;
    public float offsetY;

    float palletAnchorX;
    float palletAnchorY;


    private void Start(){

        ImageHeight = pallete.height;
        ImageWidth = pallete.width;

        palletAnchorX = 10;
        palletAnchorY = 10;// + 60;
    }

    public void ToogleColorPicker() {

        isOn = !isOn;
    }


    public void SelectColor() {

        isOn = true;
        PalleteImg.SetActive(true);


        /*
        Vector2 pickpos = Input.mousePosition;

        int aaa = Convert.ToInt32(pickpos.x - palletAnchorX - offsetX);
        int bbb = ImageHeight - Convert.ToInt32(pickpos.y - palletAnchorY - offsetY);

        color = pallete.GetPixel(aaa, bbb);

        colorRef.color = color;

    */
    }

    public void EndColorSelection() {

        PalleteImg.SetActive(false);
        isOn = false;
    }
    
    void OnGUI(){

        if (isOn && GUI.RepeatButton(new Rect(10, 10, ImageWidth, ImageHeight), pallete)){

            Vector2 pickpos = Event.current.mousePosition;

            int aaa = Convert.ToInt32(pickpos.x - palletAnchorX - offsetX);
            int bbb = ImageHeight - Convert.ToInt32(pickpos.y - palletAnchorY - offsetY);

            color = pallete.GetPixel(aaa, bbb);
            
        }

        colorRef.color = color;
    }
    
}

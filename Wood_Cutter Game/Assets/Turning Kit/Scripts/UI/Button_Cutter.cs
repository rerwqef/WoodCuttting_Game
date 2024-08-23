using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Cutter : MonoBehaviour
{
    public static List<Button_Cutter> buttons = new List<Button_Cutter>();

    public GameObject prefab;       // chisel object
    public Sprite sprite;           // picture for the button (image of the tip of the chisel)

    public bool isSelect = false;   // if you need to choose at startup

    Button button;
    Image image;


    private void Awake()
    {
        buttons.Add(this);

        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            foreach(Button_Cutter bc in buttons)
            {
                bc.ResetButton();
            }

            selectCutter();
        });

        image.sprite = sprite;

        if (isSelect)
            selectCutter();
    }

    // Deselect all buttons
    public void ResetButton()
    {
        button.interactable = true;
    }

    // Choose a chisel when you click on the button
    void selectCutter()
    {
        button.interactable = false;

        MoveCutter.moveCutter.UpdateCutter(prefab);
        MoveCutter.moveCutter.Cut();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RollButton : MonoBehaviour
{
    public TMP_Text buttonText;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set button color according to life stage
        string stage = GlobalValues.currentPlayer.GetCurrentSquare().GetComponent<Square>().stage;
        string text = "";
        if (stage == "early") {
            button.GetComponent<Image>().color = new Color32(20, 134, 0, 255);
            text = "\n(Range: 1-4)";
        } else if (stage == "mid") {
            button.GetComponent<Image>().color = new Color32(217, 173, 42, 255);
            text = "\n(Range: 1-6)";
        } else {
            button.GetComponent<Image>().color =  new Color32(43, 109, 217, 255);
            text = "\n(Range: 1-2)";
        }

        // Clarify range of roll
        buttonText.text = "ROLL" + text;
    }
}

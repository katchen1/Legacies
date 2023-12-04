using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogPanel : MonoBehaviour
{
    public TMP_Text logText;

    // Update is called once per frame
    void Update()
    {
        // Show the last five events in the panel
        string recentEvents = "";
        for (int i = 0; i < 5; i++) {
            int index = GlobalValues.events.Count - 5 + i;
            if (index >= 0) {
                if (i == 5) {
                    recentEvents += ">> ";
                }
                string e = GlobalValues.events[index];
                recentEvents += ">> " + e + "\n\n";
            }
        }
        logText.text = recentEvents;
    }
}

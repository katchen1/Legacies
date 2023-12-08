using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashText : MonoBehaviour
{
    public TMP_Text theText;
    public float flashSpeed = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Flash());
    }

    public IEnumerator Flash() {
        while (true)
        {
            // Flash to red
            theText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * flashSpeed, 1.0f));

            // Wait for the next frame
            yield return null;
        }
    }
}

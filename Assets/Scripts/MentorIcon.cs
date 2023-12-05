using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentorIcon : MonoBehaviour
{
    public GameObject mentorPanel;

    private float startY;
    private float shift;
    private bool up;

    // Start is called before the first frame update
    void Start()
    {
        startY = gameObject.transform.position.y;
        shift = 0;
        up = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the icon float up and down
        if (up) {
            shift += 0.05f;
            if (shift >= 10f) up = false;
        } else {
            shift -= 0.05f;
            if (shift <= -10f) up = true;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, startY + shift, gameObject.transform.position.z);
    }
}

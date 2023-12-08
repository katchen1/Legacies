using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public bool isStart = false;
    public bool isFinish = false;
    public bool isEvent = false;
    public bool isChallenge = false;
    public string stage = "early";

    public float GetX() {
        return gameObject.transform.position.x;
    }

    public float GetZ() {
        return gameObject.transform.position.z;
    }
}

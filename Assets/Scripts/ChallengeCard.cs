using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeCard : MonoBehaviour
{
    public string difficulty;

    public int GetPoints()
    {
        if (difficulty == "easy") {
            return 1;
        } else if (difficulty == "medium") {
            return 2;
        } else {
            return 3;
        }
    }
}

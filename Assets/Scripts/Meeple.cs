using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple : MonoBehaviour
{
    private GameObject currentSquare;
    private bool isPlayer;
    private bool isFinished = false;
    private int rankPoints = 0;
    private int challengePoints = 0;
    private int currentPathIndex = 0;
    private Vector3 positionShift;
    private string playerName;
    private string colorString;
    private Color color;
    private GameObject mentorCard;
    private bool mentorCardUsed = false;
    
    public void SetColor(Color c)
    {
        color = c;
        var capsule = gameObject.GetComponent<Renderer>();
        capsule.material.SetColor("_Color", c);
    }

    public void SetMentorCard(GameObject c)
    {
        mentorCard = c;
    }

    public GameObject GetMentorCard()
    {
        return mentorCard;
    }

    public void UseMentorCard()
    {
        mentorCardUsed = true;
    }

    public bool MentorCardUsed()
    {
        return mentorCardUsed;
    }

    public void SetColorString(string s) 
    {
        colorString = s;
    }

    public Color GetColor() 
    {
        return color;
    }

    public string GetColorString()
    {
        return colorString;
    }

    public void SetName(string s)
    {
        playerName = s;
    }

    public string GetName()
    {
        return playerName;
    }

    public void SetCurrentSquare(GameObject s)
    {
        currentSquare = s;
    }

    public GameObject GetCurrentSquare()
    {
        return currentSquare;
    }

    public void SetIsPlayer(bool b)
    {
        isPlayer = b;
    }

    public bool GetIsPlayer()
    {
        return isPlayer;
    }

    public void SetIsFinished(bool b)
    {
        isFinished = b;
    }
    
    public bool GetIsFinished()
    {
        return isFinished;
    }

    public void SetRankPoints(int i)
    {
        rankPoints = i;
    }

    public int GetRankPoints()
    {
        return rankPoints;
    }

    public void SetChallengePoints(int i)
    {
        challengePoints = i;
    }

    public void IncrementChallengePoints(int i)
    {
        challengePoints += i;
    }

    public int GetChallengePoints()
    {
        return challengePoints;
    }

    public void SetCurrentPathIndex(int i)
    {
        currentPathIndex = i;
    }

    public int GetCurrentPathIndex()
    {
        return currentPathIndex;
    }

    public void SetPositionShift(Vector3 shift)
    {
        positionShift = shift;
    }

    public Vector3 GetPositionShift()
    {
        return positionShift;
    }
}
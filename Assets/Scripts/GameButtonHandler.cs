using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtonHandler : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject rulesPanel;

    // Resume button
    public void ResumeButton()
    {
        menuPanel.SetActive(false);
    }

    // Show the rules
    public void RulesButton()
    {
        rulesPanel.SetActive(true);
    }

    // Hide the rules
    public void HideRulesButton()
    {
        rulesPanel.SetActive(false);
    }

    // Exit button
    public void ExitButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
}

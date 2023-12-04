using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject playPanel;
    public GameObject aboutPanel;
    public Slider numPlayersSlider;

    // Go to game settings
    public void PlayButton1()
    {
        playPanel.SetActive(true);
    }

    // Show the overview and link to rules
    public void AboutButton()
    {
        aboutPanel.SetActive(true);
    }

    // Go back to main menu screen
    public void CancelButton()
    {
        playPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    // Go to the main game scene
    public void PlayButton2()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Set the number of opponents
    public void SetNumPlayers()
    {
        GlobalValues.numPlayers = (int) numPlayersSlider.value;
    }
}
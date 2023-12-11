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
    public AudioSource clickAudioSource;
    public AudioSource musicAudioSource;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    // Go to game settings
    public void PlayButton1()
    {
        clickAudioSource.Play();
        playPanel.SetActive(true);
    }

    // Show the overview and link to rules
    public void AboutButton()
    {
        clickAudioSource.Play();
        aboutPanel.SetActive(true);
    }

    // Go back to main menu screen
    public void CancelButton()
    {
        clickAudioSource.Play();
        playPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    // Go to the main game scene
    public void PlayButton2()
    {
        clickAudioSource.Play();
        SceneManager.LoadScene("GameScene");
    }

    // Set the number of opponents
    public void SetNumPlayers()
    {
        GlobalValues.numPlayers = (int) numPlayersSlider.value;
    }

    // Set music
    public void SetMusic()
    {
        musicAudioSource.volume = musicToggle.isOn? 0.05f: 0f;
        GlobalValues.music = musicToggle.isOn;
    }

    // Set SFX
    public void SetSFX()
    {
        clickAudioSource.volume = sfxToggle.isOn? 1f: 0f;
        GlobalValues.sfx = sfxToggle.isOn;
    }
}
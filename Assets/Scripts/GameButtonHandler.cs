using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtonHandler : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject rulesPanel;
    public AudioSource clickAudioSource;
    public AudioSource splatAudioSource;
    public AudioSource musicAudioSource;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    // Resume button
    public void ResumeButton()
    {
        clickAudioSource.Play();
        menuPanel.SetActive(false);
    }

    // Show the rules
    public void RulesButton()
    {
        clickAudioSource.Play();
        rulesPanel.SetActive(true);
    }

    // Hide the rules
    public void HideRulesButton()
    {
        clickAudioSource.Play();
        rulesPanel.SetActive(false);
    }

    // Exit button
    public void ExitButton()
    {
        clickAudioSource.Play();
        SceneManager.LoadScene("MenuScene");
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
        splatAudioSource.volume = sfxToggle.isOn? 0.2f: 0f;
        clickAudioSource.volume = sfxToggle.isOn? 1f: 0f;
        GlobalValues.sfx = sfxToggle.isOn;
    }
}

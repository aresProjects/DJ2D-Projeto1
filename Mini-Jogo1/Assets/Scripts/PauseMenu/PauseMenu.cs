﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject options;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button back;
    [SerializeField] private Button quit;
    
    [Header("Options Menu Settings")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private SoundSettings settings;

    public static Action OnVolumeUpdate;
    
    void Start()
    {
        resumeButton.onClick.AddListener(ClosePause);
        optionsButton.onClick.AddListener(OpenOptions);
        back.onClick.AddListener(OpenPause);
        quit.onClick.AddListener(LoadMainMenu);
        
        //Options Menu (Sound Settings)
        musicToggle.onValueChanged.AddListener(UpdateMusic);
        sfxToggle.onValueChanged.AddListener(UpdateSFX);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        //Set current sound settings values
        musicToggle.isOn = settings.music;
        sfxToggle.isOn = settings.sfx;
        volumeSlider.value = settings.volume;
    }
    
    private void ClosePause()
    {
        canvas.gameObject.SetActive(false);
        GameManager.Instance.inGame = true;
        Time.timeScale = 1;
    }

    private void OpenPause()
    {
        pause.SetActive(true);
        options.SetActive(false);
    }

    private void OpenOptions()
    {
        pause.SetActive(false);
        options.SetActive(true);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    private void UpdateMusic(bool state)
    {
        settings.music = state;
    }
    
    private void UpdateSFX(bool state)
    {
        settings.sfx = state;
    }

    private void UpdateVolume(float volume)
    {
        settings.volume = volume;
        if (OnVolumeUpdate != null) OnVolumeUpdate();
    }
}

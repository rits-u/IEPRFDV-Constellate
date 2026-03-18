using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance;

public class Settings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private UnityEngine.UI.Slider volumeSlider;
    [SerializeField] private AudioSource gameAudioSource;

    [Header("Display")]
    [SerializeField] private TMP_Dropdown windowMode;
    [SerializeField] private TMP_Dropdown resolution;
    [SerializeField] private TMP_Dropdown frameRate;

    private Vector2[] resolutions ={
            new Vector2(1280, 720),
            new Vector2(1600, 900),
            new Vector2(1920, 1080)
    };
    private int[] frameRates = { 30, 60, 90, 120, 144 };
    private string[] windowModes = { "Windowed", "Fullscreen", "Borderless" };


void Start()
    {
        InitWindowModes();
        InitResolutions();
        InitFrameRates();
    }

    void InitResolutions()
    {
        var options = new List<string>();
        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].x + " x " + resolutions[i].y;
            if ((int)resolutions[i].x == Screen.width && (int)resolutions[i].y == Screen.height) currentIndex = i;
            options.Add(option);
        }
        resolution.ClearOptions();
        resolution.AddOptions(options);
        resolution.value = currentIndex;
    }

    void InitWindowModes()
    {
        List<string> options = new List<string>(windowModes);

        windowMode.ClearOptions();
        windowMode.AddOptions(options);
        windowMode.value = 0;
        windowMode.RefreshShownValue();
    }

    void InitFrameRates()
    {
        windowMode.ClearOptions();
        List<string> options = new List<string>();
        foreach(var option in frameRates)
        {
            string value = option.ToString();
            options.Add(value);
        }
        frameRate.ClearOptions();
        frameRate.AddOptions(options);
        frameRate.value = 1;
    }
    public void SetVolume()
    {
        float volume = volumeSlider.value;
        gameAudioSource.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetWindowMode()
    {
        int index = windowMode.value;
        
        FullScreenMode mode = FullScreenMode.Windowed;
        switch (index)
        {
            case 0: mode = FullScreenMode.Windowed; break;
            case 1: mode = FullScreenMode.FullScreenWindow; break;
            case 2: mode = FullScreenMode.ExclusiveFullScreen; break;
        }
        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt("WindowMode", index);
    }

    public void SetResolution()
    {
        int index = resolution.value;
        Vector2 res = resolutions[index];
        Screen.SetResolution((int)res.x, (int)res.y, Screen.fullScreenMode);
    }

    public void SetFrameRate()
    {
        int index = frameRate.value;
        Application.targetFrameRate = frameRates[index];
        PlayerPrefs.SetInt("FramerateIndex", index);
    }
}

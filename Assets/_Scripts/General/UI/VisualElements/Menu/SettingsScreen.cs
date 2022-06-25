using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SettingsScreen : FullScreenVisual
{
    AudioManager _audioManger;

    Toggle _fullScreenToggle;

    public SettingsScreen(VisualElement root)
    {
        _audioManger = AudioManager.Instance;
        Initialize(root);
        AddToClassList("menuScreen");

        AddVolumeSliders();
        AddFullScreenToggle();
        AddRadioResolutionGroup();
        AddBackButton();
    }

    void AddVolumeSliders()
    {
        Slider master = AddVolumeSlider("Master");
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        master.RegisterValueChangedCallback(MasterVolumeChange);

        Slider music = AddVolumeSlider("Music");
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        music.RegisterValueChangedCallback(MusicVolumeChange);

        Slider ambience = AddVolumeSlider("Ambience");
        ambience.value = PlayerPrefs.GetFloat("AmbienceVolume", 1);
        ambience.RegisterValueChangedCallback(AmbienceVolumeChange);

        Slider dialogue = AddVolumeSlider("Dialogue");
        dialogue.value = PlayerPrefs.GetFloat("DialogueVolume", 1);
        dialogue.RegisterValueChangedCallback(DialogueVolumeChange);

        Slider SFX = AddVolumeSlider("SFX");
        SFX.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        SFX.RegisterValueChangedCallback(SFXVolumeChange);
    }

    Slider AddVolumeSlider(string name)
    {
        //https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        VisualElement container = CreateContainer(name);
        Slider volumeSlider = new Slider();
        volumeSlider.lowValue = 0.001f;
        volumeSlider.highValue = 1f;

        container.Add(volumeSlider);
        volumeSlider.style.width = 200;
        volumeSlider.value = PlayerPrefs.GetFloat(name, 1);
        return volumeSlider;
    }

    void MasterVolumeChange(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("MasterVolume", evt.newValue);
        PlayerPrefs.Save();
        _audioManger.SetMasterVolume(evt.newValue);
    }

    void MusicVolumeChange(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
        PlayerPrefs.Save();
        _audioManger.SetMusicVolume(evt.newValue);
    }

    void AmbienceVolumeChange(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("AmbienceVolume", evt.newValue);
        PlayerPrefs.Save();
        _audioManger.SetAmbienceVolume(evt.newValue);
    }

    void DialogueVolumeChange(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("DialogueVolume", evt.newValue);
        PlayerPrefs.Save();
        _audioManger.SetDialogueVolume(evt.newValue);
    }

    void SFXVolumeChange(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("SFXVolume", evt.newValue);
        PlayerPrefs.Save();
        _audioManger.SetSFXVolume(evt.newValue);
    }

    void AddFullScreenToggle()
    {
        VisualElement container = CreateContainer("Full Screen");
        _fullScreenToggle = new Toggle();
        container.Add(_fullScreenToggle);
        SetFullScreen(PlayerPrefs.GetInt("fullScreen", 1) != 0);
        _fullScreenToggle.RegisterValueChangedCallback(FullScreenToggleClick);
    }

    void FullScreenToggleClick(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("fullScreen", (evt.newValue ? 1 : 0));
        SetFullScreen(evt.newValue);
    }

    void SetFullScreen(bool isFullScreen)
    {
        _fullScreenToggle.value = isFullScreen;
        if (isFullScreen)
            Screen.fullScreen = true;
        else
            Screen.fullScreen = false;
    }

    void AddRadioResolutionGroup()
    {
        List<string> supportedResolutions = new();
        foreach (Resolution res in Screen.resolutions)
            supportedResolutions.Add(res.ToString());

        VisualElement container = CreateContainer("Resolution");
        DropdownField dropdown = new DropdownField();
        container.Add(dropdown);

        dropdown.value = Screen.currentResolution.ToString();
        Add(dropdown);
        dropdown.choices.AddRange(supportedResolutions);
        dropdown.RegisterValueChangedCallback(SetResolution);
    }

    void SetResolution(ChangeEvent<string> evt)
    {
        string[] split = evt.newValue.Split(" x ");
        int width = int.Parse(split[0]);
        string[] split1 = split[1].Split(" @ ");
        int height = int.Parse(split1[0]);
        int hz = int.Parse(split1[1].Split("Hz")[0]);

        Screen.SetResolution(width, height, (PlayerPrefs.GetInt("fullScreen", 1) != 0), hz);
    }

    VisualElement CreateContainer(string labelText)
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);
        Label label = new Label(labelText);
        label.AddToClassList("primaryText");
        container.Add(label);
        return container;
    }
}

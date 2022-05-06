using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SettingsScreen : FullScreenVisual
{
    Toggle _fullScreenToggle;

    public SettingsScreen(VisualElement root)
    {
        Initialize(root);
        AddToClassList("menuScreen");

        AddVolumeSlider();
        AddFullScreenToggle();
        AddRadioResolutionGroup();
        AddBackButton();
    }

    void AddVolumeSlider()
    {
        VisualElement container = CreateContainer("Volume");
        SliderInt volumeSlider = new SliderInt();
        container.Add(volumeSlider);
        volumeSlider.style.width = 200;
        volumeSlider.value = PlayerPrefs.GetInt("volume", 50);
        SetVolume(PlayerPrefs.GetInt("volume", 50));
        volumeSlider.RegisterValueChangedCallback(VolumeChange);
    }

    void VolumeChange(ChangeEvent<int> evt)
    {
        PlayerPrefs.SetInt("volume", evt.newValue);
        PlayerPrefs.Save();

        SetVolume(evt.newValue);
    }

    void SetVolume(int value)
    {
        AudioListener.volume = value;
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

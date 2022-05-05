using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsUI : MonoBehaviour
{
    VisualElement _menuContainer;
    VisualElement _settingsContainer;

    SliderInt _volumeSlider;
    Toggle _fullScreenToggle;
    RadioButtonGroup _resolutionRadioGroup;

    Button _backButton;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _menuContainer = root.Q<VisualElement>("menuContainer");
        _settingsContainer = root.Q<VisualElement>("settingsContainer");

        _volumeSlider = root.Q<SliderInt>("volumeSlider");
        _volumeSlider.value = PlayerPrefs.GetInt("volume", 50);
        SetVolume(PlayerPrefs.GetInt("volume", 50));
        _volumeSlider.RegisterValueChangedCallback(VolumeChange);

        _fullScreenToggle = root.Q<Toggle>("fullScreenToggle");
        SetFullScreen(PlayerPrefs.GetInt("fullScreen", 1) != 0);
        _fullScreenToggle.RegisterValueChangedCallback(FullScreenToggleClick);

        _resolutionRadioGroup = root.Q<RadioButtonGroup>("resolutionRadioGroup");
        _resolutionRadioGroup.RegisterValueChangedCallback(SetResolution);

        _backButton = root.Q<Button>("settingsBackButton");
        _backButton.clickable.clicked += Back;
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

    void SetResolution(ChangeEvent<int> evt)
    {
        // TODO: I actually could get all supported resolutions and set them programmatically
        // Screen.resolutions

        int[] screenWidths = { 960, 1280, 1920 };

        float aspectRatio = 16 / 9f;

        Screen.SetResolution(screenWidths[evt.newValue],
                              Mathf.FloorToInt(screenWidths[evt.newValue] / aspectRatio),
                             (PlayerPrefs.GetInt("fullScreen", 1) != 0),
                             0);
    }

    void Back()
    {
        _menuContainer.style.display = DisplayStyle.Flex;
        _settingsContainer.style.display = DisplayStyle.None;
    }
}

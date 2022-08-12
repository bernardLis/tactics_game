using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SettingsScreen : FullScreenVisual
{
    GameManager _gameManager;
    AudioManager _audioManger;

    Toggle _fullScreenToggle;
    Toggle _tutorialToggle;

    VisualElement _parent;

    public SettingsScreen(VisualElement root, VisualElement parent)
    {
        _parent = parent;

        _gameManager = GameManager.Instance;
        _audioManger = AudioManager.Instance;
        Initialize(root);
        AddToClassList("menuScreen");

        VisualElement soundContainer = new VisualElement();
        soundContainer.AddToClassList("uiContainer");
        Label sound = new Label("Sound");
        sound.AddToClassList("textPrimary");
        soundContainer.Add(sound);
        Add(soundContainer);
        AddVolumeSliders(soundContainer);

        VisualElement graphicsContainer = new VisualElement();
        graphicsContainer.AddToClassList("uiContainer");
        Label graphics = new Label("Graphics");
        graphics.AddToClassList("textPrimary");
        graphicsContainer.Add(graphics);
        Add(graphicsContainer);

        AddFullScreenToggle(graphicsContainer);
        AddRadioResolutionGroup(graphicsContainer);

        if (SceneManager.GetActiveScene().name == Scenes.MainMenu)
        {
            AddPlayTutorialContainer();
            AddClearSaveButton();
        }

        AddBackButton();
    }

    void AddVolumeSliders(VisualElement parent)
    {
        Slider master = AddVolumeSlider("Master", parent);
        master.AddToClassList("volumeSlider");
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        master.RegisterValueChangedCallback(MasterVolumeChange);

        Slider music = AddVolumeSlider("Music", parent);
        music.AddToClassList("volumeSlider");
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        music.RegisterValueChangedCallback(MusicVolumeChange);

        Slider ambience = AddVolumeSlider("Ambience", parent);
        ambience.AddToClassList("volumeSlider");
        ambience.value = PlayerPrefs.GetFloat("AmbienceVolume", 1);
        ambience.RegisterValueChangedCallback(AmbienceVolumeChange);

        Slider dialogue = AddVolumeSlider("Dialogue", parent);
        dialogue.AddToClassList("volumeSlider");
        dialogue.value = PlayerPrefs.GetFloat("DialogueVolume", 1);
        dialogue.RegisterValueChangedCallback(DialogueVolumeChange);

        Slider SFX = AddVolumeSlider("SFX", parent);
        SFX.AddToClassList("volumeSlider");
        SFX.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        SFX.RegisterValueChangedCallback(SFXVolumeChange);
    }

    Slider AddVolumeSlider(string name, VisualElement parent)
    {
        //https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
        VisualElement container = CreateContainer(name);
        Slider volumeSlider = new Slider();
        volumeSlider.lowValue = 0.001f;
        volumeSlider.highValue = 1f;
        volumeSlider.style.width = 200;
        volumeSlider.value = PlayerPrefs.GetFloat(name, 1);

        container.Add(volumeSlider);
        parent.Add(container);

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

    void AddFullScreenToggle(VisualElement parent)
    {
        VisualElement container = CreateContainer("Full Screen");
        parent.Add(container);

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

    void AddRadioResolutionGroup(VisualElement parent)
    {
        List<string> supportedResolutions = new();
        foreach (Resolution res in Screen.resolutions)
            supportedResolutions.Add(res.ToString());

        VisualElement container = CreateContainer("Resolution");
        parent.Add(container);

        DropdownField dropdown = new DropdownField();
        container.Add(dropdown);

        dropdown.value = Screen.currentResolution.ToString();
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
        Label label = new Label(labelText);
        label.AddToClassList("textPrimary");
        container.Add(label);
        return container;
    }

    public override void Hide()
    {
        _parent.Focus();
        base.Hide();
    }

    void AddPlayTutorialContainer()
    {
        VisualElement container = CreateContainer("Play Tutorial");
        _tutorialToggle = new Toggle();
        _tutorialToggle.value = !_gameManager.WasTutorialPlayed;
        _tutorialToggle.RegisterValueChangedCallback(PlayTutorialToggleClick);
        container.Add(_tutorialToggle);
        Add(container);
    }

    void PlayTutorialToggleClick(ChangeEvent<bool> evt)
    {
        _tutorialToggle.value = evt.newValue;
        if (evt.newValue)
            _gameManager.SetWasTutorialPlayer(false);
        else
            _gameManager.SetWasTutorialPlayer(true);
    }

    void AddClearSaveButton()
    {

        Button button = new();
        button.AddToClassList("menuButton");
        button.text = "Clear Save Data";
        Add(button);
        ConfirmPopUp popUp = new ConfirmPopUp();
        button.clickable.clicked += () => popUp.Initialize(_root, ClearSaveData);
    }

    void ClearSaveData()
    {
        _gameManager.ClearSaveData();
    }

}

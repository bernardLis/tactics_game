using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SettingsScreen : FullScreenElement
{
    GameManager _gameManager;
    AudioManager _audioManger;

    Toggle _fullScreenToggle;
    Toggle _tutorialToggle;

    Toggle _menuEffectsToggle;
    Toggle _battleLogToggle;
    Toggle _battleHelperTextToggle;

    VisualElement _parent;


    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonUIContainer = "common__ui-container";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "settings-menu__";
    const string _ussMain = _ussClassName + "main";
    const string _ussVolumeSlider = _ussClassName + "volume-slider";


    public SettingsScreen(VisualElement root, VisualElement parent)
    {
        _gameManager = GameManager.Instance;
        _audioManger = AudioManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SettingsMenuStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _parent = parent;

        Initialize(root);
        AddToClassList(_ussMain);

        // sound
        VisualElement soundContainer = new VisualElement();
        soundContainer.AddToClassList(_ussCommonUIContainer);
        Label sound = new Label("Sound");
        sound.AddToClassList(_ussCommonTextPrimary);
        soundContainer.Add(sound);
        Add(soundContainer);
        AddVolumeSliders(soundContainer);

        // graphics
        VisualElement graphicsContainer = new VisualElement();
        graphicsContainer.AddToClassList(_ussCommonUIContainer);
        Label graphics = new Label("Graphics");
        graphics.AddToClassList(_ussCommonTextPrimary);
        graphicsContainer.Add(graphics);
        Add(graphicsContainer);

        AddFullScreenToggle(graphicsContainer);
        AddRadioResolutionGroup(graphicsContainer);

        // UI
        VisualElement uiOptionsContainer = new VisualElement();
        uiOptionsContainer.AddToClassList(_ussCommonUIContainer);
        Label uiOptionsLabel = new Label("UI Options");
        uiOptionsLabel.AddToClassList(_ussCommonTextPrimary);
        uiOptionsContainer.Add(uiOptionsLabel);
        Add(uiOptionsContainer);
        AddUIOptions(uiOptionsContainer);

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
        master.AddToClassList(_ussVolumeSlider);
        master.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        master.RegisterValueChangedCallback(MasterVolumeChange);

        Slider music = AddVolumeSlider("Music", parent);
        music.AddToClassList(_ussVolumeSlider);
        music.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        music.RegisterValueChangedCallback(MusicVolumeChange);

        Slider ambience = AddVolumeSlider("Ambience", parent);
        ambience.AddToClassList(_ussVolumeSlider);
        ambience.value = PlayerPrefs.GetFloat("AmbienceVolume", 1);
        ambience.RegisterValueChangedCallback(AmbienceVolumeChange);

        Slider dialogue = AddVolumeSlider("Dialogue", parent);
        dialogue.AddToClassList(_ussVolumeSlider);
        dialogue.value = PlayerPrefs.GetFloat("DialogueVolume", 1);
        dialogue.RegisterValueChangedCallback(DialogueVolumeChange);

        Slider SFX = AddVolumeSlider("SFX", parent);
        SFX.AddToClassList(_ussVolumeSlider);
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
        label.AddToClassList(_ussCommonTextPrimary);
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
        //_tutorialToggle.value = !_gameManager.WasTutorialPlayed;
        _tutorialToggle.RegisterValueChangedCallback(PlayTutorialToggleClick);
        container.Add(_tutorialToggle);
        Add(container);
    }

    void PlayTutorialToggleClick(ChangeEvent<bool> evt)
    {
        _tutorialToggle.value = evt.newValue;
       // if (evt.newValue)
       //     _gameManager.SetWasTutorialPlayed(false);
       // else
      //      _gameManager.SetWasTutorialPlayed(true);
    }

    void AddClearSaveButton()
    {
        ConfirmPopUp popUp = new ConfirmPopUp();
        MyButton button = new("Clear Save Data", _ussCommonMenuButton, () => popUp.Initialize(_root, ClearSaveData));
        Add(button);
    }

    void AddUIOptions(VisualElement parent)
    {
        VisualElement menuEffectsToggleContainer = CreateContainer("Disable Menu Transition Effects");
        parent.Add(menuEffectsToggleContainer);
        _menuEffectsToggle = new Toggle();
        menuEffectsToggleContainer.Add(_menuEffectsToggle);
        ToggleMenuEffects(PlayerPrefs.GetInt("HideMenuEffects", 0) != 0);
        _menuEffectsToggle.RegisterValueChangedCallback(MenuEffectsToggleClick);

        VisualElement battleLogToggleContainer = CreateContainer("Hide Battle Log");
        parent.Add(battleLogToggleContainer);
        _battleLogToggle = new Toggle();
        battleLogToggleContainer.Add(_battleLogToggle);
        ToggleBattleLog(PlayerPrefs.GetInt("HideBattleLog", 0) != 0);
        _battleLogToggle.RegisterValueChangedCallback(BattleLogToggleClick);

        VisualElement battleHelperToggleContainer = CreateContainer("Hide Battle Helper Text");
        parent.Add(battleHelperToggleContainer);
        _battleHelperTextToggle = new Toggle();
        battleHelperToggleContainer.Add(_battleHelperTextToggle);
        ToggleBattleHelperText(PlayerPrefs.GetInt("HideBattleHelperText", 0) != 0);
        _battleHelperTextToggle.RegisterValueChangedCallback(BattleHelperTextToggleClick);
    }

    void MenuEffectsToggleClick(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("HideMenuEffects", (evt.newValue ? 1 : 0));
        ToggleMenuEffects(evt.newValue);
    }

    void BattleLogToggleClick(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("HideBattleLog", (evt.newValue ? 1 : 0));
        ToggleBattleLog(evt.newValue);
    }

    void BattleHelperTextToggleClick(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("HideBattleHelperText", (evt.newValue ? 1 : 0));
        ToggleBattleHelperText(evt.newValue);
    }

    void ToggleMenuEffects(bool hide)
    {
        _menuEffectsToggle.value = hide;

        if (_gameManager == null)
            return;
        _gameManager.SetHideMenuEffects(hide);
    }

    void ToggleBattleLog(bool hide)
    {
        _battleLogToggle.value = hide;

        if (BattleUI.Instance == null)
            return;
        BattleUI.Instance.ToggleBattleLog(hide);
    }

    void ToggleBattleHelperText(bool hide)
    {
        _battleHelperTextToggle.value = hide;

        if (BattleUI.Instance == null)
            return;
        BattleUI.Instance.ToggleBattleHelperText(hide);
    }

    void ClearSaveData() { _gameManager.ClearSaveData(); }

}

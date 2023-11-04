using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuBannerChooser : MonoBehaviour
{
    const string _ussCommonButtonArrow = "common__button-arrow";
    const string _ussCommonTextHeader = "common__text-header";

    const string _ussClassName = "main-menu__";
    const string _ussBannerChoiceContainer = _ussClassName + "banner-choice-container";

    GameManager _gameManager;

    [SerializeField] List<GameObject> _poles;
    [SerializeField] List<GameObject> _flags;
    [SerializeField] List<Material> _colors;

    int _poleIndex = 0;
    Label _poleName;

    int _flagIndex = 0;
    Label _flagName;

    int _colorIndex = 0;
    Label _colorName;

    VisualElement _root;

    VisualElement _bannerContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = MainMenu.Instance.Root;
        _bannerContainer = _root.Q<VisualElement>("bannerContainer");

        SetupBannerButtons();
    }

    void SetupBannerButtons()
    {
        Label label = new($"<b>Your Banner: </b>");
        label.AddToClassList(_ussCommonTextHeader);
        _bannerContainer.Add(label);

        SetupPoleButtons();
        SetupFlagButtons();
        SetupColorButtons();
    }

    void SetupPoleButtons()
    {
        VisualElement container = new();
        container.AddToClassList(_ussBannerChoiceContainer);

        MyButton leftButton = new("<", _ussCommonButtonArrow, callback: PreviousPole);

        _poleName = new(_poles[_poleIndex].gameObject.name);
        _poleName.AddToClassList(_ussCommonTextHeader);

        MyButton rightButton = new(">", _ussCommonButtonArrow, callback: NextPole);

        container.Add(leftButton);
        container.Add(_poleName);
        container.Add(rightButton);

        _bannerContainer.Add(container);
    }

    void PreviousPole()
    {
        _poles[_poleIndex].SetActive(false);
        _poleIndex--;
        if (_poleIndex < 0)
            _poleIndex = _poles.Count - 1;
        _poles[_poleIndex].SetActive(true);
        _poleName.text = _poles[_poleIndex].gameObject.name;
        UpdateBannerPrefab();
    }

    void NextPole()
    {
        _poles[_poleIndex].SetActive(false);
        _poleIndex++;
        if (_poleIndex >= _poles.Count)
            _poleIndex = 0;
        _poles[_poleIndex].SetActive(true);
        _poleName.text = _poles[_poleIndex].gameObject.name;
        UpdateBannerPrefab();
    }

    void SetupFlagButtons()
    {
        VisualElement container = new();
        container.AddToClassList(_ussBannerChoiceContainer);

        MyButton leftButton = new("<", _ussCommonButtonArrow, callback: PreviousFlag);

        _flagName = new(_flags[_flagIndex].gameObject.name);
        _flagName.AddToClassList(_ussCommonTextHeader);

        MyButton rightButton = new(">", _ussCommonButtonArrow, callback: NextFlag);

        container.Add(leftButton);
        container.Add(_flagName);
        container.Add(rightButton);

        _bannerContainer.Add(container);
    }

    void PreviousFlag()
    {
        _flags[_flagIndex].SetActive(false);
        _flagIndex--;
        if (_flagIndex < 0)
            _flagIndex = _flags.Count - 1;

        _flags[_flagIndex].SetActive(true);
        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];
        _flagName.text = _flags[_flagIndex].name;
        UpdateBannerPrefab();
    }

    void NextFlag()
    {
        _flags[_flagIndex].SetActive(false);
        _flagIndex++;
        if (_flagIndex >= _flags.Count)
            _flagIndex = 0;

        _flags[_flagIndex].SetActive(true);
        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];
        _flagName.text = _flags[_flagIndex].name;
        UpdateBannerPrefab();
    }

    void SetupColorButtons()
    {
        VisualElement container = new();
        container.AddToClassList(_ussBannerChoiceContainer);

        MyButton leftButton = new("<", _ussCommonButtonArrow, callback: PreviousColor);
        _colorName = new(_colors[_colorIndex].name);
        _colorName.AddToClassList(_ussCommonTextHeader);

        MyButton rightButton = new(">", _ussCommonButtonArrow, callback: NextColor);
        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];

        container.Add(leftButton);
        container.Add(_colorName);
        container.Add(rightButton);

        _bannerContainer.Add(container);
    }

    void PreviousColor()
    {
        _colorIndex--;
        if (_colorIndex < 0)
            _colorIndex = _colors.Count - 1;

        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];
        _colorName.text = _colors[_colorIndex].name;
        UpdateBannerPrefab();
    }

    void NextColor()
    {
        _colorIndex++;
        if (_colorIndex >= _colors.Count)
            _colorIndex = 0;

        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];
        _colorName.text = _colors[_colorIndex].name;
        UpdateBannerPrefab();
    }

    void UpdateBannerPrefab()
    {
        _gameManager.BannerPrefab.GetComponent<BannerSetter>().SetBanner(_poleIndex, _flagIndex, _colorIndex);
    }

}


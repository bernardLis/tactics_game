using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuBannerChooser : MonoBehaviour
{
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
        _root = MainMenu.Instance.Root;
        _bannerContainer = _root.Q<VisualElement>("bannerContainer");

        SetupBannerButtons();
    }

    void SetupBannerButtons()
    {
        Label label = new Label("Your Banner: ");
        _bannerContainer.Add(label);

        SetupPoleButtons();
        SetupFlagButtons();
        SetupColorButtons();
    }

    void SetupPoleButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        MyButton leftButton = new("<", callback: PreviousPole);
        _poleName = new(_poles[_poleIndex].gameObject.name);
        MyButton rightButton = new(">", callback: NextPole);

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
    }

    void NextPole()
    {
        _poles[_poleIndex].SetActive(false);
        _poleIndex++;
        if (_poleIndex >= _poles.Count)
            _poleIndex = 0;
        _poles[_poleIndex].SetActive(true);
        _poleName.text = _poles[_poleIndex].gameObject.name;
    }

    void SetupFlagButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        MyButton leftButton = new("<", callback: PreviousFlag);
        _flagName = new(_flags[_flagIndex].gameObject.name);
        MyButton rightButton = new(">", callback: NextFlag);

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
    }

    void SetupColorButtons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        MyButton leftButton = new("<", callback: PreviousColor);
        _colorName = new(_colors[_colorIndex].name);
        MyButton rightButton = new(">", callback: NextColor);
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
    }

    void NextColor()
    {
        _colorIndex++;
        if (_colorIndex >= _colors.Count)
            _colorIndex = 0;

        _flags[_flagIndex].GetComponent<Renderer>().material = _colors[_colorIndex];
        _colorName.text = _colors[_colorIndex].name;
    }
}


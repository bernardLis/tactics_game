using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class HeroCreation : MonoBehaviour
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;

    VisualElement _root;

    TextField _nameField;

    Label _portraitTextLabel;
    VisualElement _portrait;

    VisualElement _portraitButtonContainer;
    VisualElement _elementChoiceContainer;

    MyButton _previousPortraitButton;
    MyButton _nextPortraitButton;

    VisualElement _submitButtonContainer;
    MyButton _submitButton;

    List<HeroPortrait> _heroPortraits = new();
    int _currentPortraitIndex = 0;

    Element _chosenElement;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _nameField = _root.Q<TextField>("nameField");

        _portraitTextLabel = _root.Q<Label>("portraitTextLabel");
        _portrait = _root.Q<VisualElement>("portrait");
        _portraitButtonContainer = _root.Q<VisualElement>("portraitButtonContainer");
        _elementChoiceContainer = _root.Q<VisualElement>("elementChoiceContainer");

        _submitButtonContainer = _root.Q<VisualElement>("submitButtonContainer");

        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        NameFieldSetup();
        PortraitSetup();
        CreatePortraitButtons();

        ElementChoiceElement elementChoiceElement = new();
        elementChoiceElement.OnElementChosen += (element) => _chosenElement = element;
        _elementChoiceContainer.Add(elementChoiceElement);

        CreateSubmitButton();
    }

    void NameFieldSetup()
    {
        if (Random.value > 0.5f)
            _nameField.value = _gameManager.HeroDatabase.GetRandomNameFemale();
        else
            _nameField.value = _gameManager.HeroDatabase.GetRandomNameMale();
    }

    void PortraitSetup()
    {
        _heroPortraits = new(_gameManager.HeroDatabase.GetAllPortraits());
        _heroPortraits = _heroPortraits.OrderBy(x => Random.value).ToList();

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[0].Sprite);
    }

    void CreatePortraitButtons()
    {
        _previousPortraitButton = new("<-", _ussCommonMenuButton, PreviousPortrait);
        _nextPortraitButton = new("->", _ussCommonMenuButton, NextPortrait);

        _portraitButtonContainer.Add(_previousPortraitButton);
        _portraitButtonContainer.Add(_nextPortraitButton);
    }

    void PreviousPortrait()
    {
        _currentPortraitIndex--;
        if (_currentPortraitIndex < 0)
            _currentPortraitIndex = _heroPortraits.Count - 1;

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[_currentPortraitIndex].Sprite);
    }

    void NextPortrait()
    {
        _currentPortraitIndex++;
        if (_currentPortraitIndex >= _heroPortraits.Count)
            _currentPortraitIndex = 0;

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[_currentPortraitIndex].Sprite);
    }

    void CreateSubmitButton()
    {
        _submitButton = new("Submit", _ussCommonMenuButton, Submit);
        _submitButtonContainer.Add(_submitButton);
    }

    void Submit()
    {
        if (_chosenElement == null)
        {
            Helpers.DisplayTextOnElement(_root, _submitButton, "Choose an element first!", Color.red);
            return;
        }

        CreateHero();
        StartGame();
        _gameManager.SaveJsonData();
    }

    void CreateHero()
    {
        Debug.Log($"Creating hero: {_nameField.value}");

        // TODO: an effect would be nice.
        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation(_nameField.value, _heroPortraits[_currentPortraitIndex], _chosenElement);
        _gameManager.PlayerHero = newChar;
    }

    void StartGame()
    {
        _gameManager.SelectedBattle.Opponent.Army.Clear();
        _gameManager.SelectedBattle.Opponent.Army = new(_gameManager.HeroDatabase.GetStartingArmy(_chosenElement.StrongAgainst).ArmyGroups);

        Debug.Log($"Starting game");
        _gameManager.StartGame();
    }
}

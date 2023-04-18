using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class HeroCreation : MonoBehaviour
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;

    TextField _nameField;

    Label _portraitTextLabel;
    VisualElement _portrait;

    VisualElement _portraitButtonContainer;
    MyButton _previousPortraitButton;
    MyButton _nextPortraitButton;

    VisualElement _submitButtonContainer;
    MyButton _submitButton;

    List<HeroPortrait> _heroPortraits = new();
    int _currentPortraitIndex = 0;

    bool _isPlayerCreated;
    Vector2 _playerMapPosition = new(-8.5f, -6.5f);

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _nameField = root.Q<TextField>("nameField");

        _portraitTextLabel = root.Q<Label>("portraitTextLabel");
        _portrait = root.Q<VisualElement>("portrait");
        _portraitButtonContainer = root.Q<VisualElement>("portraitButtonContainer");

        _submitButtonContainer = root.Q<VisualElement>("submitButtonContainer");

        NameFieldSetup();
        PortraitSetup();
        CreatePortraitButtons();
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
        CreateHero(_playerMapPosition);
        _isPlayerCreated = true;

        
        StartGame();
        _gameManager.SaveJsonData();
    }

    void CreateHero(Vector2 mapPosition)
    {
        Debug.Log($"Creating hero: {_nameField.value}");

        // TODO: an effect would be nice.
        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation(_nameField.value, _heroPortraits[_currentPortraitIndex], mapPosition);

        if (_isPlayerCreated)
        {
            _gameManager.FriendHero = newChar;
            return;
        }
        _gameManager.PlayerHero = newChar;
    }

    void StartGame()
    {
        Debug.Log($"Starting game");
        _gameManager.StartGame();
    }
}

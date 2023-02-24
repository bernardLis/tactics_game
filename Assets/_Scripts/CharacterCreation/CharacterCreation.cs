using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class CharacterCreation : MonoBehaviour
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

    List<CharacterPortrait> _characterPortraits = new();
    int _currentPortraitIndex = 0;

    bool _isPlayerCreated;

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
            _nameField.value = _gameManager.GameDatabase.CharacterDatabase.GetRandomNameFemale();
        else
            _nameField.value = _gameManager.GameDatabase.CharacterDatabase.GetRandomNameMale();
    }

    void PortraitSetup()
    {
        _characterPortraits = new(_gameManager.GameDatabase.CharacterDatabase.GetAllPortraits());
        _characterPortraits = _characterPortraits.OrderBy(x => Random.value).ToList();

        _portrait.style.backgroundImage = new StyleBackground(_characterPortraits[0].Sprite);
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
            _currentPortraitIndex = _characterPortraits.Count - 1;

        _portrait.style.backgroundImage = new StyleBackground(_characterPortraits[_currentPortraitIndex].Sprite);
    }

    void NextPortrait()
    {
        _currentPortraitIndex++;
        if (_currentPortraitIndex >= _characterPortraits.Count)
            _currentPortraitIndex = 0;

        _portrait.style.backgroundImage = new StyleBackground(_characterPortraits[_currentPortraitIndex].Sprite);
    }

    void CreateSubmitButton()
    {
        _submitButton = new("Submit", _ussCommonMenuButton, Submit);
        _submitButtonContainer.Add(_submitButton);
    }

    void Submit()
    {
        if (_isPlayerCreated)
        {
            CreateCharacter();
            StartGame();
            _gameManager.SaveJsonData();
            return;
        }

        CreateCharacter();
        _isPlayerCreated = true;

        _nameField.label = "What's your friend's name:";
        _nameField.value = "Fren";

        _portraitTextLabel.text = "How do they look like?";
        _currentPortraitIndex = Random.Range(0, _characterPortraits.Count);
        _portrait.style.backgroundImage = new StyleBackground(_characterPortraits[_currentPortraitIndex].Sprite);

    }

    void CreateCharacter()
    {
        Debug.Log($"Creating character: {_nameField.value}");
        // TODO: an effect would be nice.
        Character newChar = ScriptableObject.CreateInstance<Character>();
        newChar.CreateFromCharacterCreation(_nameField.value, _characterPortraits[_currentPortraitIndex]);

        if (_isPlayerCreated)
        {
            Debug.Log($"creating fren");
            _gameManager.FriendCharacter = newChar;
            return;
        }
        _gameManager.PlayerCharacter = newChar;
    }

    void StartGame()
    {
        Debug.Log($"Starting game");
        _gameManager.StartGame();
    }



}

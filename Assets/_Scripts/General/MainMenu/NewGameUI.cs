using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameUI : MonoBehaviour
{

    GameManager _gameManager;

    TextField _nameField;
    Button _confirmButton;
    Button _backButton;

    VisualElement _menuContainer;
    VisualElement _newGameScreen;

    void Start()
    {
        _gameManager = GameManager.Instance;

        var root = GetComponent<UIDocument>().rootVisualElement;

        _nameField = root.Q<TextField>("newGameName");
        _confirmButton = root.Q<Button>("newGameConfirmButton");
        _backButton = root.Q<Button>("newGameBackButton");

        _menuContainer = root.Q<VisualElement>("menuContainer");
        _newGameScreen = root.Q<VisualElement>("newGameContainer");

        _confirmButton.clickable.clicked += StartNewGame;
        _backButton.clickable.clicked += Back;
    }

    void StartNewGame()
    {
        string txt = _nameField.text;
        if (txt == null)
            return; //TODO: display a tooltip that it can't be null;

        // new save
        string guid = System.Guid.NewGuid().ToString();
        FileManager.CreateFile(guid);
        PlayerPrefs.SetString("lastSave", guid);
        PlayerPrefs.Save();

        _gameManager.StartNewGame(guid, txt);
    }

    void Back()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.Flex;
        _newGameScreen.style.display = DisplayStyle.None;
    }


}

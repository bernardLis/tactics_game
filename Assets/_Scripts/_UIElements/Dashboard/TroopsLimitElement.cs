using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TroopsLimitElement : ElementWithTooltip
{
    GameManager _gameManager;

    VisualElement _animationContainer;
    VisualElement _countContainer;

    bool _isGeneral;

    public TroopsLimitElement(bool isGeneral = true, string text = null)
    {
        _isGeneral = isGeneral;

        _gameManager = GameManager.Instance;
        if (isGeneral)
        {
            _gameManager.OnTroopsLimitChanged += OnTroopsLimitChanged;
            _gameManager.OnCharacterAddedToTroops += OnCharacterAddedToTroops;
        }

        style.flexDirection = FlexDirection.Row;
        AddToClassList("textPrimary");

        _animationContainer = new();
        _animationContainer.style.width = 50;
        _animationContainer.style.height = 50;

        Sprite[] animationSprites = _gameManager.GameDatabase.TroopsElementAnimationSprites;
        _animationContainer.Add(new AnimationElement(animationSprites, 100, true));
        Add(_animationContainer);

        _countContainer = new();
        Add(_countContainer);
        UpdateCountContainer(text);
    }

    void OnCharacterAddedToTroops(Character character) { UpdateCountContainer(); }

    void OnTroopsLimitChanged(int change) { UpdateCountContainer(); }

    void UpdateCountContainer(string text = null)
    {
        _countContainer.Clear();
        Label l = new();
        _countContainer.Add(l);
        _countContainer.style.justifyContent = Justify.Center;

        if (_isGeneral)
            l.text = $"{_gameManager.PlayerTroops.Count} / {_gameManager.TroopsLimit}";
        else
            l.text = text;
    }
}

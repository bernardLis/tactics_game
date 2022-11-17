using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TroopsLimitVisualElement : VisualWithTooltip
{
    GameManager _gameManager;

    VisualElement _animationContainer;
    VisualElement _countContainer;

    Sprite[] _animationSprites;
    IVisualElementScheduledItem _animationScheduler;
    int _animationSpriteIndex = 0;

    public TroopsLimitVisualElement()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnCharacterAddedToTroops += OnCharacterAddedToTroops;
        _animationSprites = _gameManager.GameDatabase.TroopsElementAnimationSprites;

        style.flexDirection = FlexDirection.Row;
        AddToClassList("textPrimary");

        _animationContainer = new();
        _animationContainer.style.width = 32;
        _animationContainer.style.height = 32;
        Add(_animationContainer);

        _countContainer = new();
        Add(_countContainer);
        UpdateCountContainer();

        _animationScheduler = _animationContainer.schedule.Execute(CharacterAnimation).Every(100);
    }

    void OnCharacterAddedToTroops(Character character) { UpdateCountContainer(); }

    void UpdateCountContainer()
    {
        _countContainer.Clear();
        Label l = new($"{_gameManager.PlayerTroops.Count} / {_gameManager.TroopsLimit}");
        _countContainer.Add(l);
    }

    void CharacterAnimation()
    {
        _animationContainer.style.backgroundImage = new StyleBackground(_animationSprites[_animationSpriteIndex]);
        _animationSpriteIndex++;
        if (_animationSpriteIndex == _animationSprites.Length)
            _animationSpriteIndex = 0;
    }

}

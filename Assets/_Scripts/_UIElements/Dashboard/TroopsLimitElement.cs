using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TroopsLimitElement : ElementWithTooltip
{
    GameManager _gameManager;

    VisualElement _animationContainer;
    VisualElement _countContainer;

    int _fontSize;

    const string _ussCommonTextPrimary = "common__text-primary";

    public TroopsLimitElement(string text, int fontSize = 0)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        style.flexDirection = FlexDirection.Row;
        AddToClassList(_ussCommonTextPrimary);

        if (fontSize != 0)
            _fontSize = fontSize;

        _animationContainer = new();
        _animationContainer.style.width = 50;
        _animationContainer.style.height = 50;

        Sprite[] animationSprites = _gameManager.GameDatabase.TroopsElementAnimationSprites;
        AnimationElement el = new AnimationElement(animationSprites, 100, true);
        el.PlayAnimation();
        _animationContainer.Add(el);
        Add(_animationContainer);

        _countContainer = new();
        Add(_countContainer);
        UpdateCountContainer(text, Color.white);
    }

    string GetTroopsCount() { return $"{_gameManager.PlayerTroops.Count} / {_gameManager.TroopsLimit}"; }

    public void OnCharacterAddedToTroops(Character character) { UpdateCountContainer(GetTroopsCount(), Color.white); }

    public void OnTroopsLimitChanged(int change) { UpdateCountContainer(GetTroopsCount(), Color.white); }

    public void UpdateCountContainer(string text, Color color)
    {
        _countContainer.Clear();
        Label l = new();
        l.style.color = color;
        _countContainer.Add(l);
        _countContainer.style.justifyContent = Justify.Center;
        l.text = text;
        l.style.marginBottom = 0;
        l.style.paddingBottom = 0;
        
        if (_fontSize != 0)
            l.style.fontSize = _fontSize;
    }
}

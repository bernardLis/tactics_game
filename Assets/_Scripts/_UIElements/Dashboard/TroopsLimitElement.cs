using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TroopsLimitElement : ElementWithTooltip
{
    GameManager _gameManager;

    VisualElement _animationContainer;
    VisualElement _countContainer;

    const string _ussCommonTextPrimary = "common__text-primary";


    public TroopsLimitElement(string text)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        style.flexDirection = FlexDirection.Row;
        AddToClassList(_ussCommonTextPrimary);

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
        UpdateCountContainer(text);
    }

    string GetTroopsCount() { return $"{_gameManager.PlayerTroops.Count} / {_gameManager.TroopsLimit}"; }

    public void OnCharacterAddedToTroops(Character character) { UpdateCountContainer(GetTroopsCount()); }

    public void OnTroopsLimitChanged(int change) { UpdateCountContainer(GetTroopsCount()); }

    void UpdateCountContainer(string text)
    {
        _countContainer.Clear();
        Label l = new();
        _countContainer.Add(l);
        _countContainer.style.justifyContent = Justify.Center;
        l.text = text;
    }
}

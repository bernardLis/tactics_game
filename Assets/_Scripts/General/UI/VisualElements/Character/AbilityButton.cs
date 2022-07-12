using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : Button
{
    VisualElement _icon;
    Label _keyTooltip;

    public string Key;
    public Ability Ability;

    VisualElement _animationElement;
    int _currentAnimIndex;
    Sprite[] _animationSprites;

    public AbilityButton(Ability ability, string key, VisualElement root)
    {
        Ability = ability;
        Key = key;

        style.backgroundColor = ability.HighlightColor;

        _icon = new();
        _icon.AddToClassList("abilityButtonIcon");
        _icon.style.backgroundImage = ability.Icon.texture;

        _keyTooltip = new(key);
        _keyTooltip.AddToClassList("primaryText");

        Add(_icon);
        Add(_keyTooltip);
    }
}

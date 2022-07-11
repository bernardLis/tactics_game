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

        Debug.Log($"name: {ability.name}");
        // animation
        if (ability.UIAnimationSprites.Length == 0)
            return;
        _animationElement = new VisualElement();
        _animationElement.style.position = Position.Absolute;

        root.Add(_animationElement);

        _currentAnimIndex = 0;
        _animationSprites = ability.UIAnimationSprites;
        schedule.Execute(Animate).Every(50);
    }

    void Animate()
    {
        // TODO: need to destroy the element when this ability buttons is destroyed
        _animationElement.style.left = this.resolvedStyle.left;
        _animationElement.style.top = this.resolvedStyle.top;

        _animationElement.style.backgroundImage = new StyleBackground(_animationSprites[_currentAnimIndex]);
        _currentAnimIndex++;
        if (_currentAnimIndex >= _animationSprites.Length) // am I losing 1 frame here? 
            _currentAnimIndex = 0;
    }
}

using UnityEngine.UIElements;

public class AbilityButton : Button
{
    VisualElement _icon;
    Label _keyTooltip;

    public string Key;
    public Ability Ability;

    public AbilityButton(Ability ability, string key)
    {
        Ability = ability;
        Key = key;

        _icon = new();
        _icon.AddToClassList("abilityButtonIcon");
        _icon.style.backgroundImage = ability.Icon.texture;

        _keyTooltip = new(key);

        Add(_icon);
        Add(_keyTooltip);
    }
}

using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : ElementWithSound
{
    AbilityIcon _icon;

    public string Key;
    public Ability Ability;

    VisualElement _overlay;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "ability-button__";
    const string _ussMain = _ussClassName + "main";
    const string _ussOverlay = _ussClassName + "overlay";

    public AbilityButton(Ability ability, string key = null) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Ability = ability;
        Key = key;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _icon = new AbilityIcon(ability, key);
        Add(_icon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityInfoElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "entity-info__";
    const string _ussMain = _ussClassName + "main";

    protected GameManager _gameManager;

    public EntityInfoElement(BattleEntity be)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null) styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityInfoStyles);
        if (ss != null) styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        Label name = new(be.Entity.EntityName);
        name.style.fontSize = 32;
        name.style.unityFontStyleAndWeight = FontStyle.Bold;
        name.style.position = Position.Absolute;

        Color c = _gameManager.GameDatabase.GetColorByName("Health").Color;
        ResourceBarElement bar = new(c, "Health", be.Entity.CurrentHealth,
                totalStat: be.Entity.MaxHealth);


        bar.Add(name);
        bar.HideText();

        bar.style.backgroundImage = null;
        bar.style.minWidth = 300;
        bar.style.height = 50;
        bar.style.opacity = 0.8f;

        bar.ResourceBar.style.height = Length.Percent(100);
        bar.ResourceBar.style.width = Length.Percent(100);
        bar.ResourceBar.style.marginLeft = Length.Percent(0);
        bar.ResourceBar.style.marginRight = Length.Percent(0);

        bar.MissingBar.style.height = Length.Percent(100);

        Add(bar);
    }
}

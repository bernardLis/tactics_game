using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityInfoElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-entity-info__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    public BattleEntityInfoElement(BattleEntity be)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleEntityInfoStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        Label name = new(be.Creature.Name);
        name.style.fontSize = 32;
        name.style.unityFontStyleAndWeight = FontStyle.Bold;

        IntVariable totalHealth = ScriptableObject.CreateInstance<IntVariable>();
        totalHealth.SetValue((int)be.Creature.Health);
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue((int)be.GetCurrentHealth());
        be.OnHealthChanged += (float newVal) => currentHealth.SetValue((int)newVal);
        ResourceBarElement bar = new(Helpers.GetColor("healthBarRed"), "health", totalIntVar: totalHealth, currentIntVar: currentHealth);

        name.style.position = Position.Absolute;

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

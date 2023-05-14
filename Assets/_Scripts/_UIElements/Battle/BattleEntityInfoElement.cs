using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityInfoElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-entity-info__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopContainer = _ussClassName + "top-container";

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

        VisualElement container = new();
        container.AddToClassList(_ussTopContainer);
        Add(container);

        ElementalElement el = new(be.ArmyEntity.Element);
        Label name = new(be.ArmyEntity.Name);
        name.style.fontSize = 32;
        name.style.unityFontStyleAndWeight = FontStyle.Bold;

        container.Add(el);
        container.Add(name);

        IntVariable totalHealth = ScriptableObject.CreateInstance<IntVariable>();
        totalHealth.SetValue((int)be.ArmyEntity.Health);
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue((int)be.GetCurrentHealth());
        be.OnHealthChanged += (float newVal) => currentHealth.SetValue((int)newVal);
        ResourceBarElement bar = new(Helpers.GetColor("healthBarRed"), "health", totalIntVar: totalHealth, currentIntVar: currentHealth);

        Add(bar);
    }
}

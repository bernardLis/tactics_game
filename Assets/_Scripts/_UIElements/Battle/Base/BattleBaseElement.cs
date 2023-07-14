using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleBaseElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-base__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    Base _base;

    public event Action OnClosed;

    public BattleBaseElement(Base baseObj)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleBaseStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _base = baseObj;

        foreach (BaseUpgrade upg in _base.AllBaseUpgrades)
        {
            if (upg is BaseUpgradeLives)
            {
                BaseLivesElement livesElement = new(upg as BaseUpgradeLives);
                Add(livesElement);
                continue;
            }
            BaseUpgradeElement upgElement = new BaseUpgradeElement(upg);
            Add(upgElement);
        }

        ContinueButton continueButton = new ContinueButton(callback: Close);
        Add(continueButton);
    }

    void Close()
    {
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
                {
                    OnClosed?.Invoke();
                    RemoveFromHierarchy();
                });
    }
}

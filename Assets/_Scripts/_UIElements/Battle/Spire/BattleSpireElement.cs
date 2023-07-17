using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleSpireElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "spire__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    Spire _spire;

    public event Action OnClosed;

    public BattleSpireElement(Spire spire)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SpireStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _spire = spire;

        foreach (Storey upg in _spire.Storeys)
        {
            if (upg is StoreyLives)
            {
                StoreyLivesElement livesElement = new(upg as StoreyLives);
                Add(livesElement);
                continue;
            }
            SpireUpgradeElement upgElement = new SpireUpgradeElement(upg);
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

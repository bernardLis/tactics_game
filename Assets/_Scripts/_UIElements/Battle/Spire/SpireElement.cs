using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class SpireElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "spire__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";
    const string _ussUpgradesTitle = _ussClassName + "upgrades-title";
    const string _ussUpgradesContainer = _ussClassName + "upgrades-container";

    GameManager _gameManager;

    Spire _spire;

    VisualElement _content;

    public event Action<Storey> OnStoreyPurchased;
    public event Action OnClosed;
    public SpireElement(Spire spire)
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

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        _spire = spire;

        AddLivesElement();
        AddSpireUpgrades();

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void AddLivesElement()
    {
        StoreyLivesElement livesElement = new(_spire.StoreyLives);
        _content.Add(livesElement);
    }

    void AddSpireUpgrades()
    {
        Label upgradesTitle = new("Spire Upgrades");
        upgradesTitle.AddToClassList(_ussUpgradesTitle);
        _content.Add(upgradesTitle);

        VisualElement upgradesContainer = new();
        upgradesContainer.AddToClassList(_ussUpgradesContainer);
        _content.Add(upgradesContainer);

        foreach (Storey upg in _spire.Storeys)
        {
            SpireUpgradeElement upgElement = new SpireUpgradeElement(upg);
            upgElement.OnPurchased += () => OnStoreyPurchased?.Invoke(upg);
            upgradesContainer.Add(upgElement);
        }
    }

    void Close()
    {
        DOTween.To(x => style.opacity = x, style.opacity.value, 0, 0.5f).SetUpdate(true);
        DOTween.To(x => _content.style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
                {
                    OnClosed?.Invoke();
                    RemoveFromHierarchy();
                });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class StoreyTroopsElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "storey-troops__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";
    const string _ussTreeContainer = _ussClassName + "tree-container";
    const string _ussArrowLabel = _ussClassName + "arrow-label";

    const string _ussUpgradesTitle = _ussClassName + "upgrades-title";
    const string _ussUpgradesContainer = _ussClassName + "upgrades-container";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleHeroManager _battleHeroManager;

    StoreyTroops _storey;

    VisualElement _content;

    VisualElement _topContainer;
    VisualElement _bottomContainer;

    TroopsLimitElement _troopsLimitElement;
    List<StoreyUpgradeElement> _maxTroopsTreeElements = new();

    public event Action OnClosed;
    public StoreyTroopsElement(StoreyTroops storey)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyTroopsStyles);
        if (ss != null)
            styleSheets.Add(ss);
        _battleManager = BattleManager.Instance;
        _battleHeroManager = _battleManager.GetComponent<BattleHeroManager>();

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _storey = storey;

        _content = new();
        _content.AddToClassList(_ussContent);
        Add(_content);

        AddTroopsLimitElement();

        _topContainer = new();
        _topContainer.AddToClassList(_ussTreeContainer);
        _content.Add(_topContainer);

        _bottomContainer = new();
        _content.Add(_bottomContainer);


        // upgrade troops limit
        AddMaxTroopsTree();
        // resurrect dead creatures
        AddResurrectDeadCreatures();

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void AddTroopsLimitElement()
    {
        VisualElement container = new();
        container.style.width = Length.Percent(100);
        container.style.alignItems = Align.Center;
        _content.Add(container);

        _troopsLimitElement = new($"{0}/{0}", 36);
        container.Add(_troopsLimitElement);
        UpdateTroopsLimit();
    }


    void AddMaxTroopsTree()
    {
        for (int i = 0; i < _storey.MaxTroopsTree.Count; i++)
        {
            if (i > 0)
            {
                Label arrow = new("--->");
                arrow.AddToClassList(_ussArrowLabel);
                _topContainer.Add(arrow);
            }

            StoreyUpgradeElement el = new(_storey.MaxTroopsTree[i]);
            _topContainer.Add(el);
            _maxTroopsTreeElements.Add(el);

            if (i != _storey.CurrentMaxTroopsLevel + 1)
                el.SetEnabled(false);

            el.OnPurchased += MaxLivesUpgradePurchased;
        }
    }

    void MaxLivesUpgradePurchased(StoreyUpgrade storeyUpgrade)
    {
        _maxTroopsTreeElements[_storey.CurrentMaxTroopsLevel + 1].SetEnabled(false);

        _storey.CurrentLimit.SetValue(_storey.MaxTroopsTree[_storey.CurrentMaxTroopsLevel + 1].Value);
        _storey.CurrentMaxTroopsLevel++;

        if (_storey.CurrentMaxTroopsLevel < _storey.MaxTroopsTree.Count - 1)
            _maxTroopsTreeElements[_storey.CurrentMaxTroopsLevel + 1].SetEnabled(true);

        UpdateTroopsLimit();
    }

    void UpdateTroopsLimit()
    {
        _troopsLimitElement.UpdateCountContainer(
                $"{_gameManager.PlayerHero.CreatureArmy.Count}/{_storey.CurrentLimit.Value}"
                , Color.white);
    }

    void AddResurrectDeadCreatures()
    {
        if (_battleManager.KilledPlayerEntities.Count == 0) return;

        Label title = new("Resurrect dead creatures");
        title.AddToClassList(_ussUpgradesTitle);
        _bottomContainer.Add(title);

        VisualElement deadEntitiesContainer = new();
        deadEntitiesContainer.style.flexDirection = FlexDirection.Row;
        _bottomContainer.Add(deadEntitiesContainer);

        List<BattleEntity> deadPlayerEntities = new(_battleManager.KilledPlayerEntities);
        foreach (BattleEntity be in deadPlayerEntities)
        {
            if (be.Entity is not Creature) continue;

            VisualElement container = new();
            deadEntitiesContainer.Add(container);

            EntityIcon icon = new(be.Entity);
            container.Add(icon);

            PurchaseButton pb = new(be.Entity.Level * 200, callback: () =>
            {
                _battleManager.KilledPlayerEntities.Remove(be);
                _battleHeroManager.AddCreature((Creature)be.Entity);
            });
            container.Add(pb);
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

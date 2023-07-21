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

    const string _ussTitle = _ussClassName + "title";
    const string _ussUpgradesContainer = _ussClassName + "upgrades-container";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleHeroManager _battleHeroManager;

    StoreyTroops _storey;

    VisualElement _content;

    VisualElement _topContainer;
    VisualElement _middleContainer;
    VisualElement _bottomContainer;

    Label _creatureTierLabel;
    StoreyUpgradeTreeElement _creatureTierTreeElement;

    TroopsLimitElement _troopsLimitElement;
    StoreyUpgradeTreeElement _maxTroopsTreeElement;

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

        AddCreatureTierLabel();
        _topContainer = new();
        _topContainer.AddToClassList(_ussTreeContainer);
        _content.Add(_topContainer);

        AddTroopsLimitElement();
        _middleContainer = new();
        _middleContainer.AddToClassList(_ussTreeContainer);
        _content.Add(_middleContainer);

        _bottomContainer = new();
        _content.Add(_bottomContainer);

        _creatureTierTreeElement = new(_storey.CreatureTierTree);
        _topContainer.Add(_creatureTierTreeElement);

        _maxTroopsTreeElement = new(_storey.MaxTroopsTree);
        _middleContainer.Add(_maxTroopsTreeElement);

        AddResurrectDeadCreatures();

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void AddCreatureTierLabel()
    {
        VisualElement container = new();
        container.style.width = Length.Percent(100);
        container.style.alignItems = Align.Center;
        _content.Add(container);

        _creatureTierLabel = new($"Creature tier: {0}");
        _creatureTierLabel.AddToClassList(_ussTitle);
        container.Add(_creatureTierLabel);
        _storey.CreatureTierTree.CurrentValue.OnValueChanged += UpdateCreatureTierLabel;
        UpdateCreatureTierLabel(default);
    }

    void UpdateCreatureTierLabel(int bla)
    {
        _creatureTierLabel.text = $"Creature tier: {_storey.CreatureTierTree.CurrentValue.Value}";
    }
    /*
        void AddCreatureTierTree()
        {
            for (int i = 0; i < _storey.CreatureTierTree.Count; i++)
            {
                if (i > 0)
                {
                    Label arrow = new("--->");
                    arrow.AddToClassList(_ussArrowLabel);
                    _topContainer.Add(arrow);
                }

                StoreyUpgradeElement el = new(_storey.CreatureTierTree[i]);
                _topContainer.Add(el);
                _creatureTierTreeElements.Add(el);

                if (i != _storey.CurrentCreatureTierTreeLevel + 1)
                    el.SetEnabled(false);

                el.OnPurchased += CreatureTierUpgradePurchased;
            }
        }

        void CreatureTierUpgradePurchased(StoreyUpgrade storeyUpgrade)
        {
            _creatureTierTreeElements[_storey.CurrentCreatureTierTreeLevel + 1].SetEnabled(false);

            _storey.CurrentCreatureTier.SetValue(_storey.CreatureTierTree[_storey.CurrentCreatureTierTreeLevel + 1].Value);
            _storey.CurrentCreatureTierTreeLevel++;

            if (_storey.CurrentCreatureTierTreeLevel < _storey.CreatureTierTree.Count - 1)
                _creatureTierTreeElements[_storey.CurrentCreatureTierTreeLevel + 1].SetEnabled(true);
        }
        */

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
    /*

        void AddMaxTroopsTree()
        {
            for (int i = 0; i < _storey.MaxTroopsTree.Count; i++)
            {
                if (i > 0)
                {
                    Label arrow = new("--->");
                    arrow.AddToClassList(_ussArrowLabel);
                    _middleContainer.Add(arrow);
                }

                StoreyUpgradeElement el = new(_storey.MaxTroopsTree[i]);
                _middleContainer.Add(el);
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
        */

    void UpdateTroopsLimit()
    {
        _troopsLimitElement.UpdateCountContainer(
                $"{_gameManager.PlayerHero.CreatureArmy.Count}/{_storey.MaxTroopsTree.CurrentValue.Value}"
                , Color.white);
    }

    void AddResurrectDeadCreatures()
    {
        if (_battleManager.KilledPlayerEntities.Count == 0) return;

        Label title = new("Resurrect dead creatures");
        title.AddToClassList(_ussTitle);
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

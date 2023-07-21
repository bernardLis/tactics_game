using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class StoreyManaElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "storey-mana__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContent = _ussClassName + "content";

    GameManager _gameManager;
    BattleManager _battleManager;
    BattleHeroManager _battleHeroManager;

    StoreyMana _storey;

    VisualElement _content;

    Label _title;

    StoreyUpgradeTreeElement _manaBankCapacityTreeElement;
    StoreyUpgradeTreeElement _manaPerTurnTreeElement;

    StoreyUpgradeElement _getBankMana;

    public event Action OnClosed;
    public StoreyManaElement(StoreyMana storey)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyManaStyles);
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

        _title = new();
        _content.Add(_title);
        UpdateTitle(default);

        _manaBankCapacityTreeElement = new(_storey.ManaBankCapacityTree);
        _manaPerTurnTreeElement = new(_storey.ManaPerTurnTree);
        _content.Add(_manaBankCapacityTreeElement);
        _content.Add(_manaPerTurnTreeElement);

        _storey.ManaBankCapacityTree.CurrentValue.OnValueChanged += UpdateTitle;
        _storey.ManaPerTurnTree.CurrentValue.OnValueChanged += UpdateTitle;

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _content.Add(container);

        _getBankMana = new(_storey.GetBankMana);
        _getBankMana.UpdateTitle($"Get mana from bank ({_storey.ManaInBank.Value} mana)");
        _getBankMana.OnPurchased += GetManaFromBank;
        container.Add(_getBankMana);

        StoreyUpgradeElement el = new(_storey.DirectManaRestorationUpgrade);
        container.Add(el);

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void UpdateTitle(int bla)
    {
        _title.text = $"Mana shrine adds {_storey.ManaPerTurnTree.CurrentValue.Value} mana per 10s to bank. Mana bank capacity: {_storey.ManaBankCapacityTree.CurrentValue.Value}.";
    }

    void GetManaFromBank(StoreyUpgrade storeyUpgrade)
    {
        int manaToGet = _storey.ManaInBank.Value;
        int heroMissingMana = _battleHeroManager.Hero.BaseMana.Value - _battleHeroManager.Hero.CurrentMana.Value;
        if (heroMissingMana <= 0)
        {
            Helpers.DisplayTextOnElement(Helpers.GetRoot(this), _getBankMana, "Hero has full mana", Color.blue);
            return;
        }
        manaToGet = Mathf.Clamp(manaToGet, 0, heroMissingMana);
        _storey.ManaInBank.ApplyChange(-manaToGet);
        _getBankMana.UpdateTitle($"Get mana from bank ({_storey.ManaInBank.Value} mana)");
        _battleHeroManager.Hero.CurrentMana.ApplyChange(manaToGet);
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

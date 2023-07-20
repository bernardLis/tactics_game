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
    StoreyUpgradeTreeElement _manaTurnLengthTreeElement;

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
        _manaTurnLengthTreeElement = new(_storey.ManaTurnLengthTree);
        _content.Add(_manaBankCapacityTreeElement);
        _content.Add(_manaPerTurnTreeElement);
        _content.Add(_manaTurnLengthTreeElement);

        _storey.ManaBankCapacityTree.CurrentValue.OnValueChanged += UpdateTitle;
        _storey.ManaPerTurnTree.CurrentValue.OnValueChanged += UpdateTitle;
        _storey.ManaTurnLengthTree.CurrentValue.OnValueChanged += UpdateTitle;

        Label manaInBankLabel = new($"Mana in bank: {_storey.ManaInBank.Value}");
        _storey.ManaInBank.OnValueChanged += (int value) => manaInBankLabel.text = $"Mana in bank: {value}";
        _content.Add(manaInBankLabel);

        MyButton getManaButton = new("Get mana", callback: GetManaFromBank);
        _content.Add(getManaButton);
        
        // add button for ultimate upgrade

        ContinueButton continueButton = new ContinueButton(callback: Close);
        _content.Add(continueButton);
    }

    void UpdateTitle(int bla)
    {
        _title.text = $"Mana shrine adds {_storey.ManaPerTurnTree.CurrentValue.Value} mana per {_storey.ManaTurnLengthTree.CurrentValue.Value} seconds to bank. Mana bank capacity: {_storey.ManaBankCapacityTree.CurrentValue.Value}.";
    }

    void GetManaFromBank()
    {
        int manaToGet = _storey.ManaInBank.Value;
        manaToGet = Mathf.Clamp(manaToGet, 0, _battleHeroManager.Hero.BaseMana.Value);
        _storey.ManaInBank.ApplyChange(-manaToGet);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpiceRecycleReportElement : ReportElement
{
    DraggableAbilities _draggableAbilities;

    AbilitySlot _sellSlot;
    SpiceElement _spiceElement;
    MyButton _sellButton;
    int _dayAdded;

    const string _ussClassName = "spice-recycle__";
    const string _ussSellButton = _ussClassName + "sell-button";

    public SpiceRecycleReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        _dayAdded = _gameManager.Day;
        _draggableAbilities = _deskManager.GetComponent<DraggableAbilities>();

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.SpiceRecycleReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("Spice recycle", Color.magenta);
        AddTimer("Leaving in: ");
        _expiryTimer.OnTimerFinished += OnTimerFinished;

        Label instructions = new("Drag ability to recycle it");
        _reportContents.Add(instructions);

        _sellSlot = new();
        _sellSlot.OnAbilityAdded += OnSellAbilityAdded;
        _sellSlot.OnAbilityRemoved += OnSellAbilityRemoved;

        if (report.Ability != null)
        {
            _sellSlot.AddDraggableButtonNoDelegates(report.Ability, _draggableAbilities);
            _sellSlot.AbilityButton.RegisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);
        }

        _reportContents.Add(_sellSlot);
        _draggableAbilities.AddSlot(_sellSlot);

        _spiceElement = new(0);
        _reportContents.Add(_spiceElement);

        _sellButton = new("Recycle", _ussSellButton, Sell);
        _sellButton.AddToClassList(_ussCommonTextPrimary);
        _sellButton.SetEnabled(false);
        _reportContents.Add(_sellButton);
    }

    void OnTimerFinished()
    {
        if (_sellSlot.AbilityButton != null)
            Sell();
        DismissReport();
    }

    void BlockReportPickup(PointerDownEvent e) { e.StopImmediatePropagation(); }

    void OnSellAbilityAdded(Ability ability)
    {
        _report.Ability = ability;

        _spiceElement.ChangeAmount(ability.GetAbilityValue()); // TODO: magic 2
        _sellSlot.AbilityButton.RegisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);
        _sellButton.SetEnabled(true);
    }

    void OnSellAbilityRemoved(Ability ability)
    {
        _report.Ability = null;

        _spiceElement.ChangeAmount(0);
        _sellButton.SetEnabled(false);
    }

    void Sell()
    {
        Ability soldAbility = _sellSlot.Ability;
        _gameManager.ChangeSpiceValue(soldAbility.GetAbilityValue());

        DismissReport();
    }
}

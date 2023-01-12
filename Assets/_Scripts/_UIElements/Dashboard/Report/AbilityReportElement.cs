using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class AbilityReportElement : ReportElement
{
    AbilityButton _abilityButton;
    public AbilityReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("New Ability", Color.cyan);

        _abilityButton = new AbilityButton(report.Ability);
        AbilitySlot slot = new(_abilityButton);
        _reportContents.Add(slot);

        AddAcceptRejectButtons(AcceptAbility, RejectAbility);
    }

    void AcceptAbility()
    {
        if (_gameManager.PlayerAbilityPouch.Count >= 5) // TODO: magic 5
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, this, "No more space in pouch", Color.red);
            DOTween.Shake(() => _abilityButton.transform.position, x => _abilityButton.transform.position = x, 1f);
            return;
        }

        _deskManager.AddAbilityToEmptySlot(_abilityButton);
        DismissReport();
    }

    void RejectAbility()
    {
        DismissReport();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityReportElement : ReportElement
{
    public AbilityReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("New Ability", Color.cyan);

        AbilityButton abilityButton = new AbilityButton(report.Ability);
        AbilitySlot slot = new(abilityButton);
        _reportContents.Add(slot);

        DraggableAbilities draggables = DeskManager.Instance.GetComponent<DraggableAbilities>();
        if (draggables != null)
            draggables.AddDraggableAbility(abilityButton);

        slot.OnAbilityRemoved += OnAbilityRemoved;
    }

    void OnAbilityRemoved(AbilityButton button) { DismissReport(); }


}

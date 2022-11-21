using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecruitVisualElement : VisualElement
{
    Recruit _recruit;
    Label _daysLeftLabel;

    public RecruitVisualElement(Recruit recruit)
    {
        _recruit = recruit;
        recruit.OnDaysUntilExpiredChanged += UpdateDaysLeftLabel;

        Add(new CharacterCardMini(recruit.Character));

        _daysLeftLabel = new();
        Add(_daysLeftLabel);
        UpdateDaysLeftLabel(_recruit.DaysUntilExpired);
    }

    void UpdateDaysLeftLabel(int daysLeft)
    {
        _daysLeftLabel.text = $"Days left: {daysLeft}";
        if (daysLeft <= 0)
            _daysLeftLabel.text = $"Expired.";
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecruitElement : VisualElement
{
    const string _ussClassName = "recruit-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussWageContainer = _ussClassName + "wage-container";

    GameManager _gameManager;
    Recruit _recruit;
    Label _daysLeftLabel;

    LineTimerElement _expiryTimer;

    public RecruitElement(Recruit recruit)
    {
        _gameManager = GameManager.Instance;

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RecruitElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _recruit = recruit;

        AddToClassList(_ussMain);

        Add(new CharacterCardMini(recruit.Character));
        VisualElement wageContainer = new();
        wageContainer.AddToClassList(_ussWageContainer);
        wageContainer.Add(new Label("Expected weekly wage:"));
        wageContainer.Add(new GoldElement(recruit.Character.WeeklyWage.Value));
        Add(wageContainer);

        _daysLeftLabel = new();
        Add(_daysLeftLabel);

        if (recruit.RecruitState != RecruitState.Pending)
            return;

        float timeTotal = recruit.DateTimeExpired.GetTimeInSeconds() - recruit.DateTimeAdded.GetTimeInSeconds();
        float timeLeft = recruit.DateTimeExpired.GetTimeInSeconds() - _gameManager.GetCurrentTimeInSeconds();
        _expiryTimer = new(timeLeft, timeTotal, false, "Leaving in: ");
        Add(_expiryTimer);
        _expiryTimer.OnTimerFinished += () => recruit.UpdateRecruitState(RecruitState.Expired);
    }

    void UpdateDaysLeftLabel(int daysLeft)
    {
        _daysLeftLabel.text = $"Days left: {daysLeft}";
        if (daysLeft <= 0)
            _daysLeftLabel.text = $"Expired.";
    }
}

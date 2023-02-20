using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RaiseRequestReportElement : ReportElement
{
    const string _ussClassName = "raise-request-report__";
    const string _ussNegotiateButton = _ussClassName + "negotiate-button";

    CharacterCardMini _characterCardMini;
    GoldElement _newWage;
    Label _numberOfTriesLeftLabel;
    BarMiniGameElement _barMiniGameElement;

    int _miniGameHitCount = 0;
    int _miniGameHitLimit = 3;


    public RaiseRequestReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RaiseRequestReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("Raise Request", Color.yellow);
        AddTimer("Leaving in: ");
        _timer.OnTimerFinished += DismissReport;

        _characterCardMini = new(report.Character);
        _reportContents.Add(_characterCardMini);
        string name = Helpers.ParseScriptableObjectCloneName(report.Character.name);
        _reportContents.Add(new Label($"{name} wants more money!"));

        VisualElement wageContainer = new();
        wageContainer.style.flexDirection = FlexDirection.Row;
        wageContainer.style.alignItems = Align.Center;
        wageContainer.Add(new GoldElement(report.Character.WeeklyWage.Value));
        wageContainer.Add(new Label("---->"));
        _newWage = new(report.Character.NewWage.Value);
        wageContainer.Add(_newWage);
        _reportContents.Add(wageContainer);

        if (!_report.Character.Negotiated)
        {
            _numberOfTriesLeftLabel = new();
            _reportContents.Add(_numberOfTriesLeftLabel);
            UpdateNumberOfTriesLabel();

            _barMiniGameElement = new();
            _reportContents.Add(_barMiniGameElement);
            _barMiniGameElement.OnHit += OnBarMiniGameHit;
        }
        else
        {
            _reportContents.Add(new Label("Already negotiated."));
        }

        AddAcceptRejectButtons(Accept, Reject);
    }

    void OnBarMiniGameHit(int hit)
    {
        if (IsNegotiationStarted())
        {
            _timer.Pause();
            _report.Character.SetNegotiated(true);
            _gameManager.SaveJsonData();
        }

        _miniGameHitCount++;
        UpdateNumberOfTriesLabel();
        if (IsNegotiationLimitReached())
        {
            _timer.Resume();
            _barMiniGameElement.StopGame();
            _gameManager.SaveJsonData();
        }

        if (hit == 0)
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, _characterCardMini,
                    $"I don't see it that way", Color.red);
            return;
        }

        float negotiatedPercent = hit * Random.Range(0.03f, 0.06f);
        int negotiatedAmount = Mathf.FloorToInt(_report.Character.NewWage.Value * negotiatedPercent);
        int updatedNewWage = _report.Character.NewWage.Value - negotiatedAmount;
        _newWage.ChangeAmount(updatedNewWage);
        Helpers.DisplayTextOnElement(_deskManager.Root, _characterCardMini, $"Good point!", Color.red);
        Helpers.DisplayTextOnElement(_deskManager.Root, _newWage, $"-{negotiatedAmount}", Color.red);
        _report.Character.SetNewWage(updatedNewWage);
    }

    bool IsNegotiationStarted() { return _miniGameHitCount == 0; }
    bool IsNegotiationLimitReached() { return _miniGameHitCount >= _miniGameHitLimit; }
    void UpdateNumberOfTriesLabel() { _numberOfTriesLeftLabel.text = $"Tries left: {_miniGameHitLimit - _miniGameHitCount}"; }

    void Accept()
    {
        if (_signed)
            return;

        _report.Character.SetWeeklyWage(_report.Character.NewWage.Value);
        _gameManager.AddCharacterToTroops(_report.Character);
        _report.Character.UpdateDeskPosition(new Vector2(this.worldBound.x, this.worldBound.y));
        _deskManager.SpitCharacterOntoDesk(_report.Character);
        BaseAcceptReport();
    }

    void Reject()
    {
        if (_signed)
            return;

        BaseRejectReport();
    }

}

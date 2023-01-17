using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RaiseRequestReportElement : ReportElement
{
    GoldElement _newWage;
    MyButton _negotiateButton;

    const string _ussClassName = "raise-request-report__";
    const string _ussNegotiateButton = _ussClassName + "negotiate-button";

    public RaiseRequestReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RaiseRequestReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("Raise Request", Color.yellow);

        _reportContents.Add(new CharacterCardMini(report.Character));
        _reportContents.Add(new Label($"{report.Character.name} wants more money!"));

        VisualElement wageContainer = new();
        wageContainer.style.flexDirection = FlexDirection.Row;
        wageContainer.style.alignItems = Align.Center;
        wageContainer.Add(new GoldElement(report.Character.WeeklyWage));
        wageContainer.Add(new Label("---->"));
        _newWage = new(report.Character.NewWage);
        wageContainer.Add(_newWage);
        _reportContents.Add(wageContainer);

        _negotiateButton = new("Negotiate", _ussNegotiateButton, Negotiate);
        _reportContents.Add(_negotiateButton);

        AddAcceptRejectButtons(Accept, Reject);
    }

    void Negotiate()
    {
        if (_report.Character.TimesNegotiated >= 3)
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, _negotiateButton,
                    "No, no more negotiations.", Color.red);
            return;
        }

        int negotiatedAmount = Mathf.FloorToInt(_report.Character.NewWage * Random.Range(0.08f, 0.12f));
        int updatedNewWage = _report.Character.NewWage - negotiatedAmount;
        _newWage.ChangeAmount(updatedNewWage);
        Helpers.DisplayTextOnElement(_deskManager.Root, _newWage, $"-{negotiatedAmount}", Color.red);

        _report.Character.Negotiated();
        _report.Character.SetNewWage(updatedNewWage);
        _gameManager.SaveJsonData();
    }

    void Accept()
    {
        _report.Character.SetWeeklyWage(_report.Character.NewWage);
        _gameManager.AddCharacterToTroops(_report.Character);
        BaseAcceptReport();
    }

    void Reject() { BaseRejectReport(); }

}

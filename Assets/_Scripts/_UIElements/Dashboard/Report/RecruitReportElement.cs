using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecruitReportElement : ReportElement
{
    public RecruitReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        if (_report.Recruit.RecruitState == RecruitState.Pending)
        {
            AddHeader($"{_report.Recruit.Character.CharacterName} wants to join!", new Color(0.2f, 0.2f, 0.55f));
            AddTimer("Leaving in: ");
            _reportContents.Add(new RecruitElement(_report.Recruit));
            _timer.OnTimerFinished += RecruitExpired;
            AddAcceptRejectButtons(AcceptRecruit, RejectRecruit);
        }

        if (_report.Recruit.RecruitState == RecruitState.Expired)
        {
            AddHeader($"{_report.Recruit.Character.CharacterName} left!", Helpers.GetColor(QuestState.Expired.ToString()));
            _reportContents.Add(new RecruitElement(_report.Recruit));
            AddSignButton();
            ShowSignButton();
        }
    }

    void RecruitExpired()
    {
        _report.Recruit.UpdateRecruitState(RecruitState.Expired);
        AddHeader($"{_report.Recruit.Character.CharacterName} left!", Helpers.GetColor(QuestState.Expired.ToString()));
        RemoveAcceptRejectButtons();
        AddSignButton();
        ShowSignButton();

    }

    void AcceptRecruit()
    {
        CampBuildingTroopsLimit b = _gameManager.GetComponent<BuildingManager>().TroopsLimitBuilding;
        int limit = b.GetUpgradeByRank(b.UpgradeRank).TroopsLimit;
        if (_gameManager.GetAllCharacters().Count >= limit)
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, this, "Troops Limit Exceeded", Color.red);
            return;
        }
        _report.Recruit.Character.UpdateDeskPosition(new Vector2(this.worldBound.x, this.worldBound.y));
        _gameManager.AddCharacterToTroops(_report.Recruit.Character);
        _deskManager.SpitCharacterOntoDesk(_report.Recruit.Character);
        _report.Recruit.UpdateRecruitState(RecruitState.Resolved);
        BaseAcceptReport();
    }

    void RejectRecruit()
    {
        _report.Recruit.UpdateRecruitState(RecruitState.Resolved);
        BaseRejectReport();
    }
}

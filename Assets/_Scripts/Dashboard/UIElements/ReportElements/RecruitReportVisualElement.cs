using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecruitReportVisualElement : ReportVisualElement
{
    public RecruitReportVisualElement(VisualElement parent, Report report) : base(parent, report)
    {
        if (_report.Recruit.RecruitState == RecruitState.Pending)
        {
            AddHeader($"{_report.Recruit.Character.CharacterName} wants to join!", new Color(0.2f, 0.2f, 0.55f));
            _reportContents.Add(new RecruitVisualElement(_report.Recruit));
            _report.Recruit.OnRecruitStateChanged += OnRecruitStateChanged;
            AddAcceptRejectButtons(AcceptRecruit, RejectRecruit);
        }

        if (_report.Recruit.RecruitState == RecruitState.Expired)
        {
            AddHeader($"{_report.Recruit.Character.CharacterName} left!", Helpers.GetColor(QuestState.Expired.ToString()));
            _reportContents.Add(new RecruitVisualElement(_report.Recruit));
            AddSignButton();
            ShowSignButton();
        }
    }

    void OnRecruitStateChanged(RecruitState newState)
    {
        if (newState == RecruitState.Expired)
        {
            AddHeader($"{_report.Recruit.Character.CharacterName} left!", Helpers.GetColor(QuestState.Expired.ToString()));
            RemoveAcceptRejectButtons();
            AddSignButton();
            ShowSignButton();
        }
    }

    void AcceptRecruit()
    {
        if (_gameManager.PlayerTroops.Count >= _gameManager.TroopsLimit)
        {
            Helpers.DisplayTextOnElement(this, this, "Troops Limit Exceeded", Color.red);
            return;
        }

        _gameManager.AddCharacterToTroops(_report.Recruit.Character);
        _report.Recruit.UpdateRecruitState(RecruitState.Resolved);
        BaseAcceptReport();
    }

    void RejectRecruit()
    {
        _report.Recruit.UpdateRecruitState(RecruitState.Resolved);
        BaseRejectReport();
    }
}

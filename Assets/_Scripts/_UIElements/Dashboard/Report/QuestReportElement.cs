using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestReportElement : ReportElement
{
    public QuestReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        QuestElement q = new QuestElement(_report);
        _reportContents.Add(q);

        _report.Quest.OnQuestStateChanged += OnQuestStateChanged;
        UpdateQuestHeader();
        AddSignButton();

        if (_report.Quest.QuestState == QuestState.Expired)
            ShowSignButton();
        if (_report.Quest.QuestState == QuestState.RewardCollected)
            ShowSignButton();
    }

    void UpdateQuestHeader()
    {
        if (_report.Quest.QuestState == QuestState.Pending)
            AddHeader(_report.Quest.Title, Helpers.GetColor(QuestState.Pending.ToString()));
        if (_report.Quest.QuestState == QuestState.Delegated)
            AddHeader(_report.Quest.Title, Helpers.GetColor(QuestState.Delegated.ToString()));
        if (_report.Quest.QuestState == QuestState.Finished)
            AddHeader(_report.Quest.Title, Helpers.GetColor(QuestState.Finished.ToString()));
        if (_report.Quest.QuestState == QuestState.Expired)
            AddHeader(_report.Quest.Title, Helpers.GetColor(QuestState.Expired.ToString()));
        if (_report.Quest.QuestState == QuestState.RewardCollected)
        {
            Color col = _report.Quest.IsWon ? Helpers.GetColor("healthGainGreen") : Helpers.GetColor("damageRed");
            AddHeader(_report.Quest.Title, col);
        }
    }

    void OnQuestStateChanged(QuestState state)
    {
        UpdateQuestHeader();
        if (state == QuestState.Expired)
            ShowSignButton();
        if (state == QuestState.RewardCollected)
            ShowSignButton();
    }
}

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
            AddHeader("Quest Pending", Helpers.GetColor(QuestState.Pending.ToString()));
        if (_report.Quest.QuestState == QuestState.Delegated)
            AddHeader("Quest In Progress", Helpers.GetColor(QuestState.Delegated.ToString()));
        if (_report.Quest.QuestState == QuestState.Finished)
            AddHeader("See Quest Results", Helpers.GetColor(QuestState.Finished.ToString()));
        if (_report.Quest.QuestState == QuestState.Expired)
            AddHeader("Quest Expired", Helpers.GetColor(QuestState.Expired.ToString()));
        if (_report.Quest.QuestState == QuestState.RewardCollected)
        {
            string txt = _report.Quest.IsWon ? "Quest won!" : "Quest lost!";
            Color col = _report.Quest.IsWon ? Helpers.GetColor("healthGainGreen") : Helpers.GetColor("damageRed");
            AddHeader(txt, col);
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

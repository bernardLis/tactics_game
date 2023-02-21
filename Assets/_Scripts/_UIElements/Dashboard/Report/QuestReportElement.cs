using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestReportElement : ReportElement
{
    const string _ussClassName = "quest-report__";

    const string _ussTimerWrapper = _ussClassName + "timer-element-wrapper";
    const string _ussTimerLine = _ussClassName + "timer-element-line";
    const string _ussTimerLineDelegated = _ussClassName + "timer-element-line-delegated";

    public QuestReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.QuestReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _report.Quest.OnQuestStateChanged += OnQuestStateChanged;
        UpdateQuestHeader();

        if (_report.Quest.QuestState == QuestState.Pending)
            HandlePendingQuest();
        if (_report.Quest.QuestState == QuestState.Delegated)
            HandleDelegatedQuest();

        QuestElement q = new QuestElement(_report);
        _reportContents.Add(q);
        AddSignButton();

        if (_report.Quest.QuestState == QuestState.Expired)
            ShowSignButton();
        if (_report.Quest.QuestState == QuestState.RewardCollected)
            ShowSignButton();
    }

    void HandlePendingQuest()
    {
        AddTimer("Expires in: ");
        _timer.OnTimerFinished += OnTimerFinished;
        _timer.SetStyles(_ussTimerWrapper, _ussTimerLine);
    }

    void HandleDelegatedQuest()
    {
        float totalTime = _report.Quest.DurationSeconds;
        float remainingTime = totalTime;

        if (_timer == null) // loading
        {
            AddTimer("");
            float end = _report.Quest.DateTimeStarted.GetTimeInSeconds() + totalTime;
            remainingTime = end - _gameManager.GetCurrentTimeInSeconds();
            _timer.OnTimerFinished += OnTimerFinished;
        }

        _timer.UpdateLabel("Finished in: ");
        _timer.UpdateTimerValues(remainingTime, totalTime);
        _timer.SetStyles(_ussTimerWrapper, _ussTimerLineDelegated);
    }

    void OnTimerFinished()
    {
        if (_report.Quest.QuestState == QuestState.Pending)
            _report.Quest.UpdateQuestState(QuestState.Expired);

        if (_report.Quest.QuestState == QuestState.Delegated)
            _report.Quest.FinishQuest();
    }

    void OnQuestStateChanged(QuestState state)
    {
        UpdateQuestHeader();
        if (state == QuestState.Delegated)
            HandleDelegatedQuest();
        if (state == QuestState.Expired)
            ShowSignButton();
        if (state == QuestState.RewardCollected)
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

}

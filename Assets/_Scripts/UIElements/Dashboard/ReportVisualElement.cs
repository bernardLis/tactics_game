using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ReportVisualElement : ScrollView
{
    GameManager _gameManager;
    VisualElement _parent;
    Report _report;
    bool _isArchived;

    public event Action<ReportVisualElement> OnReportDismissed;
    public ReportVisualElement(VisualElement parent, Report report)
    {
        _gameManager = GameManager.Instance;
        _parent = parent;
        _report = report;

        AddToClassList("report");
        AddToClassList("textPrimary");

        // depending on type it will look differently
        if (report.ReportType == ReportType.Quest)
            HandleQuest();
        if (report.ReportType == ReportType.FinishedQuest)
            HandleFinishedQuest();
        if (report.ReportType == ReportType.ExpiredQuest)
            HandleExpiredQuest();
        if (report.ReportType == ReportType.Recruit)
            HandleRecruit();
        if (report.ReportType == ReportType.Text)
            HandleText();
    }

    void HandleQuest()
    {
        // report removes quest from new quests and adds it to available quests
        AddHeader("Quest", new Color(0.27f, 0.4f, 0.56f));

        Add(new QuestVisualElement(_report.Quest));
    }

    void HandleFinishedQuest()
    {
        // distinction between delegated quest and player quest
        // display the quest, the characters that partook and clickable reward
        AddHeader("Quest Finished!", new Color(0.18f, 0.2f, 0.21f));

        Label result = new();
        Add(result);
        result.text = _report.Quest.IsWon ? "Won! :)" : "Lost! :(";

        Add(new QuestVisualElement(_report.Quest, true));

        if (_report.Quest.IsWon && !_isArchived)
        {
            RewardContainer rc = new RewardContainer(_report.Quest.Reward);
            rc.OnChestOpen += OnRewardChestOpen;
            Add(rc);
        }

        VisualElement container = new();
        Add(container);

        foreach (Character character in _report.Quest.AssignedCharacters)
            container.Add(new CharacterCardExtended(character));
    }

    void OnRewardChestOpen()
    {
        _report.Quest.Reward.GetReward();
        AddSignButton();
    }

    void HandleExpiredQuest()
    {
        AddHeader("Quest Expired!", new Color(0.55f, 0.2f, 0.21f));
        Add(new QuestVisualElement(_report.Quest));
        AddSignButton();
    }

    void HandleRecruit()
    {
        AddHeader($"{_report.Recruit.CharacterName} wants to join!", new Color(0.2f, 0.2f, 0.55f));
        Add(new CharacterCardExtended(_report.Recruit));
        AddAcceptRejectButtons(AcceptRecruit, RejectRecruit);
    }

    void AcceptRecruit()
    {
        _gameManager.AddCharacterToTroops(_report.Recruit);
        BaseAcceptReport();
    }

    void RejectRecruit() { BaseRejectReport(); }

    void HandleText()
    {
        AddHeader("New Message", new Color(0.27f, 0.56f, 0.34f));

        Label text = new(_report.Text);
        text.style.fontSize = 36;
        Add(text);

        AddSignButton();
    }

    // HELPERS
    void AddHeader(string text, Color color)
    {
        Label header = new(text);
        header.style.fontSize = 48;
        header.style.backgroundColor = color;
        header.style.unityTextAlign = TextAnchor.MiddleCenter;
        Add(header);
    }

    void AddAcceptRejectButtons(Action acceptCallback, Action rejectCallback)
    {
        if (_report.IsSigned)
        {
            HandleSignedReportWithDecision();
            return;
        }

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        MyButton acceptButton = new MyButton("Accept", "menuButton", acceptCallback);
        MyButton rejectButton = new MyButton("Reject", "menuButton", rejectCallback);
        container.Add(acceptButton);
        container.Add(rejectButton);
        Add(container);
    }

    void HandleSignedReportWithDecision()
    {
        Label l = new();
        l.text = _report.WasAccepted ? $"Accepted on day {_report.DaySigned}" : $"Rejected on day {_report.DaySigned}";
        l.style.color = _report.WasAccepted ? Color.green : Color.red;
        Add(l);
    }

    void AddSignButton()
    {
        if (_report.IsSigned)
        {
            Label signed = new($"Signed on day {_report.DaySigned}");
            Add(signed);
            return;
        }

        Button sign = new();
        sign.AddToClassList("menuButton");
        sign.text = "Sign";
        Add(sign);

        sign.clickable.clicked += DismissReport;
    }

    void BaseAcceptReport()
    {
        _report.WasAccepted = true;
        DismissReport();
    }
    void BaseRejectReport()
    {
        _report.WasAccepted = false;
        DismissReport();
    }

    void DismissReport()
    {
        OnReportDismissed?.Invoke(this);
        Blur();

        _report.Sign();

        // archive report
        _gameManager.Reports.Remove(_report);
        _gameManager.ReportsArchived.Add(_report);
        _gameManager.SaveJsonData();
    }

}

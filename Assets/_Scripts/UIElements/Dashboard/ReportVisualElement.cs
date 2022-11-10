using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;
using DG.Tweening;

public class ReportVisualElement : VisualElement
{
    GameManager _gameManager;
    AudioManager _audioManager;

    VisualElement _parent;
    ScrollView _reportContents;
    VisualElement _reportShadow;
    Report _report;
    bool _isArchived;

    bool _isDragging;

    public event Action<ReportVisualElement> OnReportDismissed;
    public ReportVisualElement(VisualElement parent, Report report)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        _parent = parent;
        _report = report;

        AddToClassList("report");
        AddToClassList("textPrimary");

        _reportShadow = new();
        _reportShadow.AddToClassList("reportShadow");
        _reportShadow.style.display = DisplayStyle.None;
        Add(_reportShadow);

        _reportContents = new();
        _reportContents.AddToClassList("reportContents");
        Add(_reportContents);

        // depending on type it will look differently
        if (report.ReportType == ReportType.Quest)
            HandleQuest();
        if (report.ReportType == ReportType.FinishedQuest)
            HandleFinishedQuest();
        if (report.ReportType == ReportType.Recruit)
            HandleRecruit();
        if (report.ReportType == ReportType.Text)
            HandleText();

        parent.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        parent.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDayPassed(int day)
    {
        if (_report.ReportType == ReportType.Quest)
            HandleQuest();
    }

    void HandleQuest()
    {
        _reportContents.Clear();
        if (_report.Quest.IsExpired())
        {
            HandleExpiredQuest();
            return;
        }

        AddHeader("Quest", new Color(0.27f, 0.4f, 0.56f));
        _reportContents.Add(new QuestVisualElement(_report.Quest));
    }

    void HandleExpiredQuest()
    {
        AddHeader("Quest Expired!", new Color(0.55f, 0.2f, 0.21f));
        _reportContents.Add(new QuestVisualElement(_report.Quest));
        AddSignButton();
    }

    void HandleFinishedQuest()
    {
        // distinction between delegated quest and player quest
        // display the quest, the characters that partook and clickable reward
        AddHeader("Quest Finished!", new Color(0.18f, 0.2f, 0.21f));

        Label result = new();
        _reportContents.Add(result);
        result.text = _report.Quest.IsWon ? "Won! :)" : "Lost! :(";

        _reportContents.Add(new QuestVisualElement(_report.Quest, true));

        if (_report.Quest.IsWon && !_isArchived)
        {
            RewardContainer rc = new RewardContainer(_report.Quest.Reward);
            rc.OnChestOpen += OnRewardChestOpen;
            _reportContents.Add(rc);
        }

        VisualElement container = new();
        _reportContents.Add(container);

        foreach (Character character in _report.Quest.AssignedCharacters)
            container.Add(new CharacterCardExtended(character));
    }

    void OnRewardChestOpen()
    {
        _report.Quest.Reward.GetReward();
        AddSignButton();
    }

    void HandleRecruit()
    {
        AddHeader($"{_report.Recruit.CharacterName} wants to join!", new Color(0.2f, 0.2f, 0.55f));
        _reportContents.Add(new CharacterCardMini(_report.Recruit));
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
        _reportContents.Add(text);

        AddSignButton();
    }

    // HELPERS
    void AddHeader(string text, Color color)
    {
        Label header = new(text);
        header.style.fontSize = 48;
        header.style.backgroundColor = color;
        header.style.unityTextAlign = TextAnchor.MiddleCenter;

        header.RegisterCallback<PointerDownEvent>(OnHeaderPointerDown);

        _reportContents.Add(header);
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
        MyButton acceptButton = new MyButton(null, "acceptButton", acceptCallback);
        MyButton rejectButton = new MyButton(null, "rejectButton", rejectCallback);
        container.Add(acceptButton);
        container.Add(rejectButton);
        _reportContents.Add(container);
    }

    void HandleSignedReportWithDecision()
    {
        Label l = new();
        l.text = _report.WasAccepted ? $"Accepted on day {_report.DaySigned}" : $"Rejected on day {_report.DaySigned}";
        l.style.color = _report.WasAccepted ? Color.green : Color.red;
        _reportContents.Add(l);
    }

    void AddSignButton()
    {
        if (_report.IsSigned)
        {
            Label signed = new($"Signed on day {_report.DaySigned}");
            _reportContents.Add(signed);
            return;
        }

        MyButton signButton = new MyButton(null, "signButton", DismissReport);
        _reportContents.Add(signButton);
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

    async void DismissReport()
    {
        Blur();

        _report.Sign();
        _audioManager.PlaySFX("Stamp", Vector3.zero);

        Label signed = new($"Signed on day {_gameManager.Day}");
        _reportContents.Add(signed);
        // TODO: a better way? XD
        await Task.Delay(50);
        signed.AddToClassList("signedBefore");
        await Task.Delay(50);
        signed.ToggleInClassList("signedBefore");
        signed.AddToClassList("signedText");


        await Task.Delay(400);
        OnReportDismissed?.Invoke(this);
        _audioManager.PlaySFX("PaperFlying", Vector3.zero);

        // archive report
        _gameManager.Reports.Remove(_report);
        _gameManager.ReportsArchived.Add(_report);
        _gameManager.SaveJsonData();
    }

    /* DRAG & DROP */
    void OnHeaderPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;
        if (_isArchived)
            return;

        _isDragging = true;

        StartReportDrag(evt.position);
    }

    void StartReportDrag(Vector2 position)
    {
        BringToFront();

        AddToClassList("reportPickedUp");
        _audioManager.PlaySFX("Paper", Vector3.zero);
        _reportShadow.style.display = DisplayStyle.Flex;
        style.left = position.x - layout.width / 2;
        style.top = position.y - layout.height / 4;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        // Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        // Set the new position
        style.left = evt.position.x - layout.width / 2;
        style.top = evt.position.y - layout.height / 4;
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        _audioManager.PlaySFX("PlacingPaper", Vector3.zero);
        RemoveFromClassList("reportPickedUp");

        _reportShadow.style.display = DisplayStyle.None;
        _report.Position = new Vector2(style.left.value.value, style.top.value.value);
        _gameManager.SaveJsonData();
    }


}

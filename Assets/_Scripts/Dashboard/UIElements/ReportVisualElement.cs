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
    VisualElement _reportShadow;

    VisualElement _reportContents;
    Label _header;

    Report _report;
    VisualElement _acceptRejectContainer;
    MyButton _signButton;
    bool _isArchived;

    Vector2 _dragOffset;
    bool _isDragging;

    bool _signed;

    public event Action<ReportVisualElement> OnReportDismissed;
    public ReportVisualElement(VisualElement parent, Report report)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        _parent = parent;
        _report = report;

        AddToClassList("report");

        _reportShadow = new();
        _reportShadow.AddToClassList("reportShadow");
        _reportShadow.style.display = DisplayStyle.None;
        Add(_reportShadow);

        _reportContents = new();
        _reportContents.AddToClassList("reportContents");
        _reportContents.style.backgroundImage = new StyleBackground(report.ReportPaper.Sprite);
        Add(_reportContents);

        _header = new();
        _reportContents.Add(_header);

        // depending on type it will look differently
        if (report.ReportType == ReportType.Quest)
            HandleQuest();
        if (report.ReportType == ReportType.Recruit)
            HandleRecruit();
        if (report.ReportType == ReportType.Text)
            HandleText();
        if (report.ReportType == ReportType.CampBuilding)
            HandleCampBuilding();

        _reportContents.RegisterCallback<PointerDownEvent>(OnReportContentPointerDown);
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        parent.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        parent.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDayPassed(int day)
    {
    }

    void HandleQuest()
    {
        QuestVisualElement q = new QuestVisualElement(_report);
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

    void HandleRecruit()
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

    void HandleText()
    {
        AddHeader("New Message", new Color(0.27f, 0.56f, 0.34f));

        Label text = new(_report.Text);
        text.style.width = Length.Percent(70f);
        text.style.fontSize = 36;
        text.style.whiteSpace = WhiteSpace.Normal;
        _reportContents.Add(text);

        AddSignButton();
        ShowSignButton();
    }

    void HandleCampBuilding()
    {
        CampBuilding b = _gameManager.GetCampBuildingById(_report.CampBuildingId);
        AddHeader("Building Finished!", new Color(0.66f, 0.42f, 0.17f));

        CampBuildingVisualElement el = new CampBuildingVisualElement(b);
        el.style.backgroundImage = null; // looks weird coz report already has paper bg
        _reportContents.Add(el);

        AddSignButton();
        ShowSignButton();
    }

    // HELPERS
    void AddHeader(string text, Color color)
    {
        _header.text = text;
        _header.AddToClassList("reportHeader");
        _header.style.unityBackgroundImageTintColor = color;
    }

    void AddAcceptRejectButtons(Action acceptCallback, Action rejectCallback)
    {
        if (_report.IsSigned)
        {
            HandleSignedReportWithDecision();
            return;
        }

        _acceptRejectContainer = new();
        _acceptRejectContainer.AddToClassList("acceptRejectContainer");

        MyButton acceptButton = new MyButton(null, "acceptButton", acceptCallback);
        MyButton rejectButton = new MyButton(null, "rejectButton", rejectCallback);
        _acceptRejectContainer.Add(acceptButton);
        _acceptRejectContainer.Add(rejectButton);
        _reportContents.Add(_acceptRejectContainer);
    }

    void RemoveAcceptRejectButtons()
    {
        _acceptRejectContainer.Clear();
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

        // TODO: this could look better.
        _signButton = new MyButton(null, "signButtonContainer", DismissReport);
        VisualElement signButtonInside = new();
        signButtonInside.AddToClassList("signButton");
        _signButton.Add(signButtonInside);
        _signButton.style.visibility = Visibility.Hidden;
        _reportContents.Add(_signButton);
    }

    void ShowSignButton()
    {
        if (_signButton == null)
            return;
        _signButton.style.visibility = Visibility.Visible;
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
        // otherwise you can click multiple times if you are a quick clicker.
        if (_signed)
            return;
        _signed = true;

        _reportContents.UnregisterCallback<PointerDownEvent>(OnReportContentPointerDown);

        if (_acceptRejectContainer != null)
            _acceptRejectContainer.style.visibility = Visibility.Hidden;
        if (_signButton != null)
            _signButton.style.visibility = Visibility.Hidden;

        Blur();

        _report.Sign();
        _audioManager.PlaySFX("Stamp", Vector3.zero);

        Label signed = new($"Signed on day {_gameManager.Day}");
        _reportContents.Add(signed);

        // TODO: a better way? XD
        signed.AddToClassList("signedBefore");
        signed.style.display = DisplayStyle.None;
        await Task.Delay(50); // this makes transitions from class to class to work.
        signed.AddToClassList("signedText");
        await Task.Delay(10); // TODO: nasty nasty nasty, but without it the text appears on the bottom of the element without styling for a few frames
        signed.style.display = DisplayStyle.Flex;

        await Task.Delay(400);
        OnReportDismissed?.Invoke(this);
        _audioManager.PlaySFX("PaperFlying", Vector3.zero);

        // archive report
        _gameManager.Reports.Remove(_report);
        _gameManager.ReportsArchived.Add(_report);
        _gameManager.SaveJsonData();
    }

    /* DRAG & DROP */
    void OnReportContentPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;
        if (_isArchived)
            return;

        _isDragging = true;
        _dragOffset = new Vector2(evt.localPosition.x, evt.localPosition.y);
        _dragOffset.y += _parent.worldBound.y; // it needs to be additionally offset by nav bar height

        StartReportDrag(evt.position);
    }

    void StartReportDrag(Vector2 position)
    {
        BringToFront();

        AddToClassList("reportPickedUp");
        _audioManager.PlaySFX("Paper", Vector3.zero);
        _reportShadow.style.display = DisplayStyle.Flex;
        style.left = position.x - _dragOffset.x;
        style.top = position.y - _dragOffset.y;
    }

    void OnPointerDown(PointerDownEvent evt) { BringToFront(); }

    void OnPointerMove(PointerMoveEvent evt)
    {
        // Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        // Set the new position
        style.left = evt.position.x - _dragOffset.x;
        style.top = evt.position.y - _dragOffset.y;
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

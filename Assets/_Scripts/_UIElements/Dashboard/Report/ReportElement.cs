using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;
using DG.Tweening;

public class ReportElement : VisualElement
{
    protected GameManager _gameManager;
    protected AudioManager _audioManager;
    protected DeskManager _deskManager;

    protected VisualElement _parent;
    protected VisualElement _reportShadow;

    protected VisualElement _reportContents;
    protected Label _header;

    protected Report _report;
    protected VisualElement _acceptRejectContainer;
    protected MyButton _signButton;
    protected bool _isArchived;

    protected Vector2 _dragOffset;
    protected bool _isDragging;

    protected bool _signed;

    const string _ussClassName = "report";
    const string _ussMain = _ussClassName + "__main";
    const string _ussShadow = _ussClassName + "__shadow";
    const string _ussContents = _ussClassName + "__contents";
    const string _ussHeader = _ussClassName + "__header";
    const string _ussPickedUp = _ussClassName + "__picked-up";

    const string _ussDecisionContainer = _ussClassName + "__decision-container";
    const string _ussAccept = _ussClassName + "__accept";
    const string _ussReject = _ussClassName + "__reject";

    const string _ussSignContainer = _ussClassName + "__sign-container";
    const string _ussSignButton = _ussClassName + "__sign_button";
    const string _ussSignedText = _ussClassName + "__signed_text";
    const string _ussSignedTextBefore = _ussClassName + "__signed-text-before";

    public event Action<ReportElement> OnReportDismissed;
    public ReportElement(VisualElement parent, Report report)
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        _deskManager = DeskManager.Instance;

        _parent = parent;
        _report = report;

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ReportStyles);
        if (ss != null)
            styleSheets.Add(ss);
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        _reportShadow = new();
        _reportShadow.AddToClassList(_ussShadow);
        _reportShadow.style.display = DisplayStyle.None;
        Add(_reportShadow);

        _reportContents = new();
        _reportContents.AddToClassList(_ussContents);
        _reportContents.style.backgroundImage = new StyleBackground(report.ReportPaper.Sprite);
        Add(_reportContents);

        _header = new();
        _reportContents.Add(_header);

        _reportContents.RegisterCallback<PointerDownEvent>(OnReportContentPointerDown);
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        parent.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        parent.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected void OnDayPassed(int day)
    {
        // meant to be overwritten
    }

    // HELPERS
    protected void AddHeader(string text, Color color)
    {
        _header.text = text;
        _header.AddToClassList(_ussHeader);
        _header.style.unityBackgroundImageTintColor = color;
    }

    protected void AddAcceptRejectButtons(Action acceptCallback, Action rejectCallback)
    {
        if (_report.IsSigned)
        {
            HandleSignedReportWithDecision();
            return;
        }

        _acceptRejectContainer = new();
        _acceptRejectContainer.AddToClassList(_ussDecisionContainer);

        MyButton acceptButton = new MyButton(null, _ussAccept, acceptCallback);
        MyButton rejectButton = new MyButton(null, _ussReject, rejectCallback);
        _acceptRejectContainer.Add(acceptButton);
        _acceptRejectContainer.Add(rejectButton);
        _reportContents.Add(_acceptRejectContainer);
    }

    protected void RemoveAcceptRejectButtons() { _acceptRejectContainer.Clear(); }

    protected void HandleSignedReportWithDecision()
    {
        Label l = new();
        l.text = _report.WasAccepted ? $"Accepted on day {_report.DaySigned}" : $"Rejected on day {_report.DaySigned}";
        l.style.color = _report.WasAccepted ? Color.green : Color.red;
        _reportContents.Add(l);
    }

    protected void AddSignButton()
    {
        if (_report.IsSigned)
        {
            Label signed = new($"Signed on day {_report.DaySigned}");
            _reportContents.Add(signed);
            return;
        }

        // TODO: this could look better.
        _signButton = new MyButton(null, _ussSignContainer, DismissReportAction);
        VisualElement signButtonInside = new();
        signButtonInside.AddToClassList(_ussSignButton);
        _signButton.Add(signButtonInside);
        _signButton.style.visibility = Visibility.Hidden;
        _reportContents.Add(_signButton);
    }

    protected void ShowSignButton()
    {
        if (_signButton == null)
            return;
        _signButton.style.visibility = Visibility.Visible;
    }

    protected void BaseAcceptReport()
    {
        _report.WasAccepted = true;
        DismissReport();
    }
    protected void BaseRejectReport()
    {
        _report.WasAccepted = false;
        DismissReport();
    }

    void DismissReportAction() { DismissReport(); } // otherwise, the delegate throws errors

    protected async void DismissReport(bool SoundOn = true)
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
        if (SoundOn)
            _audioManager.PlaySFX("Stamp", Vector3.zero);

        Label signed = new($"Signed on day {_gameManager.Day}");
        _reportContents.Add(signed);

        // TODO: a better way? XD
        signed.AddToClassList(_ussSignedTextBefore);
        signed.style.display = DisplayStyle.None;
        await Task.Delay(50); // this makes transitions from class to class to work.
        signed.AddToClassList(_ussSignedText);
        await Task.Delay(10); // TODO: nasty nasty nasty, but without it the text appears on the bottom of the element without styling for a few frames
        signed.style.display = DisplayStyle.Flex;

        await Task.Delay(400);
        OnReportDismissed?.Invoke(this);
        if (SoundOn)
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

        AddToClassList(_ussPickedUp);
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
        RemoveFromClassList(_ussPickedUp);

        _reportShadow.style.display = DisplayStyle.None;
        _report.Position = new Vector2(style.left.value.value, style.top.value.value);
        _gameManager.SaveJsonData();
    }
}

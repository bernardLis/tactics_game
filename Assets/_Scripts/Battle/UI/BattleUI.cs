using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    JourneyManager _journeyManager;

    VisualElement _turnTextContainer;
    Label _turnText;

    VisualElement _battleEndContainer;
    Label _battleEndText;
    Button _backToJourney;

    string _turnTextTweenID = "turnTextTweenID";

    void Awake()
    {
        _journeyManager = JourneyManager.instance;

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _turnTextContainer = root.Q<VisualElement>("turnTextContainer");
        _turnText = root.Q<Label>("turnText");

        _battleEndContainer = root.Q<VisualElement>("battleEndContainer");
        _battleEndText = root.Q<Label>("battleEndText");
        _backToJourney = root.Q<Button>("backToJourney");

        _backToJourney.clickable.clicked += BackToJourney;

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            DisplayText("DEPLOY TROOPS");
        if (state == BattleState.EnemyTurn)
            DisplayText("TURN " + TurnManager.CurrentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            DisplayText("TURN " + TurnManager.CurrentTurn.ToString() + " - PLAYER");
        if (state == BattleState.Won)
            ShowBattleWonScreen();
        if (state == BattleState.Lost)
            ShowBattleLostScreen();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    async void DisplayText(string text)
    {
        if (DOTween.TweensById(_turnTextTweenID) != null)
        {
            Debug.Log("awaiting");
            await DOTween.TweensById(_turnTextTweenID)[0].AsyncWaitForCompletion();
        }

        _turnText.text = text;
        _turnTextContainer.style.display = DisplayStyle.Flex;
        Debug.Log($"show text{text}");

        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 1f, 2f)
            .OnComplete(HideText)
            .SetId(_turnTextTweenID);
    }

    void HideText()
    {
        Debug.Log("hide text");
        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 0f, 2f)
            .OnComplete(() => _turnTextContainer.style.display = DisplayStyle.None)
            .SetId(_turnTextTweenID);
    }


    void ShowBattleWonScreen()
    {
        ShowBattleEndScreen();
        _battleEndText.text = "WON!!";
    }

    void ShowBattleLostScreen()
    {
        ShowBattleEndScreen();
        _battleEndText.text = "LOST!!";
    }

    void ShowBattleEndScreen()
    {
        _turnTextContainer.style.display = DisplayStyle.None;

        _battleEndContainer.style.display = DisplayStyle.Flex;
        _battleEndContainer.style.opacity = 0f;
        DOTween.To(() => _battleEndContainer.style.opacity.value, x => _battleEndContainer.style.opacity = x, 1f, 2f)
            .OnComplete(() => _backToJourney.style.display = DisplayStyle.Flex);
    }

    void BackToJourney()
    {
        _journeyManager.LoadLevel("Journey");
    }
}

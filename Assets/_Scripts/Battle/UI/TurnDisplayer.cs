using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class TurnDisplayer : MonoBehaviour
{
    VisualElement _turnTextContainer;
    Label _turnText;

    string _tweenID = "tweenID";

    void Start()
    {
        // getting ui elements
        var root = GameUI.instance.GetComponent<UIDocument>().rootVisualElement;
        _turnTextContainer = root.Q<VisualElement>("turnTextContainer");
        _turnText = root.Q<Label>("turnText");

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
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    async void DisplayText(string text)
    {
        if (DOTween.TweensById(_tweenID) != null)
            await DOTween.TweensById(_tweenID)[0].AsyncWaitForCompletion();

        _turnText.text = text;
        _turnTextContainer.style.display = DisplayStyle.Flex;

        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 1f, 2f)
            .OnComplete(HideText)
            .SetId(_tweenID);
    }

    void HideText()
    {
        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 0f, 2f)
            .OnComplete(() => _turnTextContainer.style.display = DisplayStyle.None)
            .SetId(_tweenID);
    }
}

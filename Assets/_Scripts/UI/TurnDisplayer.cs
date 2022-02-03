using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class TurnDisplayer : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement turnTextContainer;
    Label turnText;

    string tweenID = "tweenID";

    void Start()
    {
        UIDocument = GameUI.instance.GetComponent<UIDocument>();

        // getting ui elements
        var root = UIDocument.rootVisualElement;
        turnTextContainer = root.Q<VisualElement>("turnTextContainer");
        turnText = root.Q<Label>("turnText");

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            DisplayText("DEPLOY TROOPS");
        if (state == BattleState.EnemyTurn)
            DisplayText("TURN " + TurnManager.currentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            DisplayText("TURN " + TurnManager.currentTurn.ToString() + " - PLAYER");
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    async void DisplayText(string _text)
    {
        if (DOTween.TweensById(tweenID) != null)
            await DOTween.TweensById(tweenID)[0].AsyncWaitForCompletion();

        turnText.text = _text;
        turnTextContainer.style.display = DisplayStyle.Flex;

        DOTween.To(() => turnTextContainer.style.opacity.value, x => turnTextContainer.style.opacity = x, 1f, 2f)
            .OnComplete(HideText)
            .SetId(tweenID);
    }

    void HideText()
    {
        DOTween.To(() => turnTextContainer.style.opacity.value, x => turnTextContainer.style.opacity = x, 0f, 2f)
            .OnComplete(() => turnTextContainer.style.display = DisplayStyle.None)
            .SetId(tweenID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class TurnDisplayer : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement turnTextContainer;
    Label turnText;

    Queue<IEnumerator> coroutineQueue = new();

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

        // coroutine queue
        //StartCoroutine(CoroutineCoordinator());
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            DisplayText("DEPLOY TROOPS");
        if (state == BattleState.EnemyTurn)
            DisplayText("TURN " + TurnManager.currentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            DisplayText("TURN " + TurnManager.currentTurn.ToString() + " - PLAYER");

        /*
        if (state == BattleState.Deployment)
            coroutineQueue.Enqueue(DisplayTurnText("DEPLOY TROOPS"));
        if (state == BattleState.EnemyTurn)
            coroutineQueue.Enqueue(DisplayTurnText("TURN " + TurnManager.currentTurn.ToString() + " - ENEMY"));
        if (state == BattleState.PlayerTurn)
            coroutineQueue.Enqueue(DisplayTurnText("TURN " + TurnManager.currentTurn.ToString() + " - PLAYER"));
    */
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
        // first, check if there is active tween, if there is, wait for it
        DOTween.To(() => turnTextContainer.style.opacity.value, x => turnTextContainer.style.opacity = x, 1f, 2f)
            .OnComplete(HideText)
            .SetId(tweenID);
        // after, hide text

    }

    void HideText()
    {

        DOTween.To(() => turnTextContainer.style.opacity.value, x => turnTextContainer.style.opacity = x, 0f, 2f)
            .OnComplete(() => turnTextContainer.style.display = DisplayStyle.None)
            .SetId(tweenID);

    }
    /*
    // https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
    // coroutine queue
    // TODO: is it performance-expensive? 
    // TODO: use dotween instead
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    IEnumerator DisplayTurnText(string _text)
    {
        turnText.text = _text;

        turnTextContainer.style.display = DisplayStyle.Flex;

        // fade in
        float waitTime = 0;
        float fadeTime = 1f;
        while (waitTime < 1)
        {
            turnTextContainer.style.opacity = waitTime;
            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }

        yield return new WaitForSeconds(1);

        // fade out
        waitTime = 0;
        fadeTime = 1f;
        while (waitTime < 1)
        {
            turnTextContainer.style.opacity = 1 - waitTime;
            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }

        turnTextContainer.style.display = DisplayStyle.None;
    }
    */
}

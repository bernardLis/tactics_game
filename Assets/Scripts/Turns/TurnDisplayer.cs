using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnDisplayer : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement turnTextContainer;
    Label turnText;

    Queue<IEnumerator> coroutineQueue = new();

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
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(DisplayTurnText("DEPLOY TROOPS"));
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if(state == BattleState.EnemyTurn)
            coroutineQueue.Enqueue(DisplayTurnText("TURN " + TurnManager.currentTurn.ToString() + " - ENEMY"));
        if(state == BattleState.PlayerTurn)
            coroutineQueue.Enqueue(DisplayTurnText("TURN " + TurnManager.currentTurn.ToString() + " - PLAYER"));
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    // https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
    // coroutine queue
    // TODO: is it performance-expensive? 
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
}

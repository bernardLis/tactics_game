using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConversationUI : MonoBehaviour
{
    VisualElement _conversationContainer;
    VisualElement _conversationPortrait;
    Label _conversationText;

    float _topPercent = 100f;
    IVisualElementScheduledItem _scheduler;

    bool _printTextCoroutineFinished = true;
    bool _isAnimationOn;

    IEnumerator _typeTextCoroutine;
    void Awake()
    {
        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;
        _conversationContainer = root.Q<VisualElement>("conversationContainer");
        _conversationPortrait = root.Q<VisualElement>("conversationPortrait");
        _conversationText = root.Q<Label>("conversationText");
    }

    public void ShowUI()
    {
        // TODO: use dotween instead
        if (_isAnimationOn)
            return;
        // https://forum.unity.com/threads/animation-via-code-examples.948161/
        // https://forum.unity.com/threads/propertydrawer-with-uielements-changes-in-array-dont-refresh-inspector.747467/
        // https://docs.unity3d.com/ScriptReference/UIElements.IVisualElementScheduledItem.html
        // set the container all the way to the bottom

        // only one can be visible.
       // GameUI.Instance.HideAllUIPanels();
        _conversationContainer.style.display = DisplayStyle.Flex;

        _conversationContainer.style.top = Length.Percent(_topPercent);
        // 'animate' it to come up 
        _isAnimationOn = true;
        _scheduler = _conversationContainer.schedule.Execute(() => AnimateConversationBoxUp()).Every(10); // ms
    }

    public void HideUI()
    {
        if (_isAnimationOn)
            return;

        _isAnimationOn = true;
        _scheduler = _conversationContainer.schedule.Execute(() => AnimateConversationBoxDown()).Every(10); // ms

    }

    void AnimateConversationBoxUp()
    {
        if (_topPercent > 75f)
        {
            _conversationContainer.style.top = Length.Percent(_topPercent);
            _topPercent--;
            return;
        }

        // TODO: idk how to destroy scheduler...
        _isAnimationOn = false;

        _scheduler.Pause();
    }

    void AnimateConversationBoxDown()
    {
        if (_topPercent < 100f)
        {
            _conversationContainer.style.top = Length.Percent(_topPercent);
            _topPercent++;
            return;
        }

        // TODO: idk how to destroy scheduler...
        _isAnimationOn = false;
        _scheduler.Pause();
        _conversationContainer.style.display = DisplayStyle.None;
    }

    public void SetPortrait(Sprite sprite)
    {
        _conversationPortrait.style.backgroundImage = new StyleBackground(sprite);
    }

    public void SetText(string text)
    {
        _conversationText.Clear();
        _conversationText.style.color = Color.white;

        if (_typeTextCoroutine != null)
            StopCoroutine(_typeTextCoroutine);

        _typeTextCoroutine = TypeText(text);
        StartCoroutine(_typeTextCoroutine);
        _printTextCoroutineFinished = false;
    }

    public void SkipTextTyping(string text)
    {
        _conversationText.style.color = Color.white;

        if (_typeTextCoroutine != null)
            StopCoroutine(_typeTextCoroutine);

        _conversationText.text = text;
        _printTextCoroutineFinished = true;
    }

    public bool IsLinePrinted()
    {
        return _printTextCoroutineFinished;
    }

    IEnumerator TypeText(string text)
    {
        _conversationText.text = "";
        char[] charArray = text.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            _conversationText.text += charArray[i];

            if (i == charArray.Length - 1)
                _printTextCoroutineFinished = true;

            yield return new WaitForSeconds(0.03f);
        }
    }


}

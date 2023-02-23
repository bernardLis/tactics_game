using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ConversationManager : Singleton<ConversationManager>
{
    AudioManager _audioManager;

    VisualElement _conversationContainer;
    VisualElement _conversationPortrait;
    Label _conversationTooltip;
    Label _conversationText;

    bool _isConversationOn;
    bool _printTextCoroutineFinished = true;

    Conversation _conversation;
    int _currentLineIndex = 0;
    string _currentText;

    IEnumerator _typeTextCoroutine;
    string _tweenID = "_conversationTooltip";

    protected override void Awake()
    {
        base.Awake();


        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;
        _conversationContainer = root.Q<VisualElement>("conversationContainer");
        _conversationPortrait = root.Q<VisualElement>("conversationPortrait");
        _conversationText = root.Q<Label>("conversationText");
        _conversationTooltip = root.Q<Label>("conversationTooltip");
    }

    void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    public async Task PlayConversation(Conversation conversation)
    {
        GameManager.Instance.GetComponent<GameUIManager>().DisableMenuButton(); // TODO: ugh...
        _conversation = conversation;
        _isConversationOn = true;
        _currentLineIndex = 0;
        PlayLine(conversation.Lines[0]);
        DOTween.To(x => _conversationTooltip.transform.scale = x * Vector3.one, 1, 1.2f, 1.5f).SetLoops(-1).SetEase(Ease.InOutSine).SetId(_tweenID);

        while (_isConversationOn)
            await Task.Yield();

        GameManager.Instance.GetComponent<GameUIManager>().EnableMenuButton(); // TODO: ugh...
    }

    void KeyPressed()
    {
        if (!_isConversationOn)
            return;

        if (!IsLinePrinted())
        {
            SkipTextTyping();
            return;
        }
        if (_currentLineIndex < _conversation.Lines.Length - 1)
        {
            _currentLineIndex++;
            PlayLine(_conversation.Lines[_currentLineIndex]);
            return;
        }

        HideUI();
        _isConversationOn = false;
    }
    /*
        void ShowUI()
        {
            quadrant = 1

            int randomX = Random.Range(2, 10);
            int randomY = Random.Range(2, 10);
            _conversationContainer.style.top = StyleKeyword.Null;
            _conversationContainer.style.bottom = StyleKeyword.Null;
            _conversationContainer.style.left = StyleKeyword.Null;
            _conversationContainer.style.right = StyleKeyword.Null;

            // [Tooltip("0: bottom left, 1: bottom right, 2: top left, 3: top right")]
            if (quadrant == 0)
            {
                _conversationContainer.style.bottom = Length.Percent(randomY);
                _conversationContainer.style.left = Length.Percent(randomX);
            }
            if (quadrant == 1)
            {
                _conversationContainer.style.bottom = Length.Percent(randomY);
                _conversationContainer.style.right = Length.Percent(randomX);
            }
            if (quadrant == 2)
            {
                _conversationContainer.style.top = Length.Percent(randomY);
                _conversationContainer.style.left = Length.Percent(randomX);
            }
            if (quadrant == 3)
            {
                _conversationContainer.style.top = Length.Percent(randomY);
                _conversationContainer.style.right = Length.Percent(randomX);
            }

            _conversationContainer.style.display = DisplayStyle.Flex;
        }
        */

    void HideUI()
    {
        DOTween.Kill(_tweenID);
        _conversationTooltip.transform.scale = Vector3.one;

        _conversationContainer.style.display = DisplayStyle.None;
        _audioManager.StopDialogue();
    }


    async void PlayLine(ConversationLine line)
    {
        // known bug )by unity(
        // ArgumentException: Event must be a StateEvent or DeltaStateEvent but is a TEXT instead
        _conversationPortrait.style.backgroundImage = new StyleBackground(line.SpeakerCharacter.Portrait.Sprite);

        float letterPrintingDelay = 0.05f;
        if (line.VO)
        {
            _audioManager.StopDialogue();
            _audioManager.PlayDialogue(line.VO);
            char[] charArray = line.Text.ToCharArray();

            float duration = line.VO.Clips[0].length * 1000; // magic 1 second ....
            letterPrintingDelay = Mathf.FloorToInt(duration) / charArray.Length;//- 10; // magic -10
            letterPrintingDelay *= 0.001f;
        }

    //  /   ShowUI();
        _currentText = line.Text;
        SetText(letterPrintingDelay);

        await Task.Delay(200); // to prevent skipping multiple steps on one click
        InputSystem.onAnyButtonPress.CallOnce((key) => KeyPressed());
    }

    public void SetText(float letterPrintingDelay)
    {
        _conversationText.Clear();
        _conversationText.style.color = Color.white;

        if (_typeTextCoroutine != null)
            StopCoroutine(_typeTextCoroutine);

        _typeTextCoroutine = TypeText(_currentText, letterPrintingDelay);
        StartCoroutine(_typeTextCoroutine);
        _printTextCoroutineFinished = false;
    }

    async void SkipTextTyping()
    {
        if (_typeTextCoroutine != null)
            StopCoroutine(_typeTextCoroutine);

        _conversationText.text = _currentText;
        _printTextCoroutineFinished = true;

        await Task.Delay(200); // to prevent skipping multiple steps on one click
        InputSystem.onAnyButtonPress.CallOnce((key) => KeyPressed());
    }

    bool IsLinePrinted()
    {
        return _printTextCoroutineFinished;
    }

    IEnumerator TypeText(string text, float letterPrintingDelay)
    {
        _conversationText.text = "";
        char[] charArray = text.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            _conversationText.text += charArray[i];

            if (i == charArray.Length - 1)
                _printTextCoroutineFinished = true;

            yield return new WaitForSeconds(letterPrintingDelay);
        }
    }
}

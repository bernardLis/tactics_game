using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour
{
    const string _ussClassName = "cutscene__";
    const string _ussBackground = _ussClassName + "background";
    const string _ussLineBox = _ussClassName + "line-box";
    const string _ussLineLabel = _ussClassName + "line-label";

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;
    BattleEndManager _battleEndManager;

    [SerializeField] Conversation _introConversation;

    HeroCardMini _currentSpeaker;
    ConversationLine _currentLine;
    List<HeroCardMini> _cardsInConversation = new();

    VisualElement _root;
    VisualElement _lineBox;
    Label _lineLabel;

    string _shakyShakyTweenId = "ShakyShakyTweenId";

    VisualElement _cutsceneContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _battleManager = GetComponent<BattleManager>();
        _battleEndManager = GetComponent<BattleEndManager>();

        _root = GetComponent<UIDocument>().rootVisualElement;

        _battleEndManager.OnBattleResultShown += OnBattleResultShown;


    }

    void OnBattleResultShown()
    {
        _battleEndManager.BattleResult.HideContent();

        _cutsceneContainer = new();
        _cutsceneContainer.style.position = Position.Absolute;
        _cutsceneContainer.style.width = Length.Percent(100);
        _cutsceneContainer.style.height = Length.Percent(100);

        _battleEndManager.BattleResult.Add(_cutsceneContainer);


        _lineBox = new();
        _lineBox.AddToClassList(_ussLineBox);
        _lineBox.style.visibility = Visibility.Hidden;
        _cutsceneContainer.Add(_lineBox); // HERE: not to root, either to battle result or on top of battle result 

        _lineLabel = new();
        _lineLabel.AddToClassList(_ussLineLabel);
        _lineBox.Add(_lineLabel);

        _introConversation.Initialize();
        PlayConversation(_introConversation);
    }

    IEnumerator PlayConversation(Conversation conversation)
    {
        _lineBox.style.visibility = Visibility.Visible;

        foreach (ConversationLine line in conversation.Lines)
        {
            _currentLine = line;
            yield return HandleLineBox(line);
            yield return TypeText(line);
        }

        _lineBox.style.visibility = Visibility.Hidden;
    }

    IEnumerator HandleLineBox(ConversationLine line)
    {
        _lineBox.style.visibility = Visibility.Hidden;
        _lineLabel.text = line.ParsedText;

        yield return new WaitForSeconds(0.1f);

        _lineLabel.style.width = _lineLabel.resolvedStyle.width;
        _lineLabel.style.height = _lineLabel.resolvedStyle.height;

        foreach (HeroCardMini c in _cardsInConversation)
        {
            if (c.Hero == line.SpeakerHero)
            {
                HandleSpeaker(c);
                UpdateBoxPosition(c);
            }
        }
    }

    void HandleSpeaker(HeroCardMini card)
    {
        _currentSpeaker = card;
        float duration = 1f;
        if (_currentLine.VO != null)
            duration = _currentLine.VO.Clips[0].length;

        DOTween.To(x => card.transform.scale = x * Vector3.one, 1, 1.5f, 0.5f).OnComplete(() =>
        {

            float strength = Random.Range(2, 5);
            int vibrato = Random.Range(5, 15);
            float randomness = Random.Range(50, 150);

            DOTween.Shake(() => card.transform.position, x => card.transform.position = x, duration,
                    strength, vibrato, randomness)
                        .SetLoops(-1)
                        .SetId(_shakyShakyTweenId);
        });
    }

    void UpdateBoxPosition(HeroCardMini card)
    {
        _lineBox.BringToFront();
        float elWidth = _lineBox.resolvedStyle.width;
        float elHeight = _lineBox.resolvedStyle.height;
        float x = card.worldBound.xMin;
        float y = card.worldBound.yMin;

        if (x + elWidth + card.resolvedStyle.width > Screen.width)
            _lineBox.style.left = x - elWidth;
        else
            _lineBox.style.left = x + card.resolvedStyle.width;

        if (y - elHeight - card.resolvedStyle.height < 0)
            _lineBox.style.top = y - elHeight;

        else
            _lineBox.style.top = y - elHeight;
    }


    IEnumerator TypeText(ConversationLine line)
    {
        _lineBox.style.visibility = Visibility.Visible;
        _lineLabel.text = "";

        char[] charArray = line.ParsedText.ToCharArray();

        float delay = 0.1f;
        if (line.VO != null)
        {
            delay = line.VO.Clips[0].length / charArray.Length;
            _audioManager.PlayDialogue(line.VO);
        }

        for (int i = 0; i < charArray.Length; i++)
        {
            _lineLabel.text += charArray[i];
            yield return new WaitForSeconds(delay);
        }

        DOTween.To(x => _currentSpeaker.transform.scale = x * Vector3.one, _currentSpeaker.transform.scale.x, 1, 0.5f);
        DOTween.Kill(_shakyShakyTweenId);
        yield return new WaitForSeconds(0.5f);
    }
}

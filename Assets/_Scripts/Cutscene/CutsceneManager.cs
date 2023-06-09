using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    const string _ussClassName = "cutscene__";
    const string _ussBackground = _ussClassName + "background";
    const string _ussLineBox = _ussClassName + "line-box";
    const string _ussLineLabel = _ussClassName + "line-label";

    GameManager _gameManager;
    AudioManager _audioManager;

    Cutscene _currentCutscene;
    HeroCardMini _currentSpeaker;
    List<HeroCardMini> _cardsInConversation = new();

    VisualElement _root;
    VisualElement _lineBox;
    Label _lineLabel;

    string _shakyShakyTweenId = "ShakyShakyTweenId";

    VisualElement _cutsceneContainer;

    public event Action<Cutscene> OnCutsceneFinished;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public void Initialize(VisualElement root)
    {
        _root = root;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CutsceneStyles);
        if (ss != null)
            _root.styleSheets.Add(ss);

        _cutsceneContainer = new();
        _cutsceneContainer.style.position = Position.Absolute;
        _cutsceneContainer.style.width = Length.Percent(100);
        _cutsceneContainer.style.height = Length.Percent(100);
        _cutsceneContainer.style.left = 0;
        _cutsceneContainer.style.top = 0;
        _root.Add(_cutsceneContainer);

        _lineBox = new();
        _lineBox.AddToClassList(_ussLineBox);
        _lineBox.style.visibility = Visibility.Hidden;
        _cutsceneContainer.Add(_lineBox);

        _lineLabel = new();
        _lineLabel.AddToClassList(_ussLineLabel);
        _lineBox.Add(_lineLabel);

        _cardsInConversation.Clear();

        _gameManager.GetComponent<PlayerInput>().actions["Continue"].performed
                += evt => SkipCutscene();
    }

    void SkipCutscene()
    {
        if (this == null) return;
        StopAllCoroutines();
        CutsceneLine lastLine = _currentCutscene.Lines[_currentCutscene.Lines.Length - 1];
        StartCoroutine(InitializeSpeaker(lastLine));
        StartCoroutine(HandleLineBox(lastLine, true));
        _lineBox.style.visibility = Visibility.Visible;
        _lineLabel.text = lastLine.ParsedText;
        _audioManager.StopDialogue();
        DOTween.KillAll(); // TODO: risky but works for now
        OnCutsceneFinished?.Invoke(_currentCutscene);
    }

    public void PlayCutscene(Cutscene cutscene)
    {
        Debug.Log($"Playing cutscene {cutscene.name}");

        _currentCutscene = cutscene;
        cutscene.Initialize();
        StartCoroutine(RunCutscene(cutscene));
    }

    /*
            _battleEndManager.OnBattleResultShown += () =>
        {
            if (_gameManager.BattleNumber != 2) return;
            _battleEndManager.BattleResult.OnRewardContainerClosed += RivalIntro;
        };

        void RivalIntro()
        {
            // only before the third battle

            _battleEndManager.BattleResult.HideContent();

            _cutsceneContainer = new();
            _cutsceneContainer.style.position = Position.Absolute;
            _cutsceneContainer.style.width = Length.Percent(100);
            _cutsceneContainer.style.height = Length.Percent(100);

            _battleEndManager.BattleResult.Add(_cutsceneContainer);

            _lineBox = new();
            _lineBox.AddToClassList(_ussLineBox);
            _lineBox.style.visibility = Visibility.Hidden;
            _cutsceneContainer.Add(_lineBox);

            _lineLabel = new();
            _lineLabel.AddToClassList(_ussLineLabel);
            _lineBox.Add(_lineLabel);

            _cardsInConversation.Clear();
            _introConversation.Initialize();

            Battle battle = ScriptableObject.CreateInstance<Battle>();
            battle.Opponent = _gameManager.RivalHero;
            _gameManager.SelectedBattle = battle;

            // HERE: never gets past this line
            StartCoroutine(PlayCutscene(_introConversation));
            OnCutsceneFinished += (c) =>
            {
                _gameManager.LoadScene(Scenes.Battle);
            };

        }
        */

    IEnumerator RunCutscene(Cutscene cutscene)
    {
        foreach (CutsceneLine line in cutscene.Lines)
        {
            yield return InitializeSpeaker(line);
            yield return HandleLineBox(line);
            yield return TypeText(line);
        }
        OnCutsceneFinished?.Invoke(cutscene);
    }

    IEnumerator InitializeSpeaker(CutsceneLine line)
    {
        foreach (HeroCardMini card in _cardsInConversation)
        {
            if (card.Hero == line.Speaker.SpeakerHero)
                yield break;
        }

        HeroCardMini newCard = new(line.Speaker.SpeakerHero);
        newCard.style.position = Position.Absolute;
        newCard.style.opacity = 0;
        // parse position [Tooltip("0 - left, 1 - middle, 2 right | 0 - up, 1 - middle, 2 - right")]
        if (line.Speaker.SpeakerPosition.x == 0)
            newCard.style.left = Length.Percent(10);
        else if (line.Speaker.SpeakerPosition.x == 1)
            newCard.style.left = Length.Percent(50);
        else if (line.Speaker.SpeakerPosition.x == 2)
            newCard.style.left = Length.Percent(90);

        if (line.Speaker.SpeakerPosition.y == 0)
            newCard.style.top = Length.Percent(20);
        else if (line.Speaker.SpeakerPosition.y == 1)
            newCard.style.top = Length.Percent(50);
        else if (line.Speaker.SpeakerPosition.y == 2)
            newCard.style.top = Length.Percent(90);

        _cutsceneContainer.Add(newCard);
        _cardsInConversation.Add(newCard);
        yield return DOTween.To(x => newCard.style.opacity = x, 0, 1, 0.5f).WaitForCompletion();
    }

    IEnumerator HandleLineBox(CutsceneLine line, bool isSkipped = false)
    {
        _lineBox.style.visibility = Visibility.Hidden;
        _lineLabel.text = line.ParsedText;

        yield return new WaitForSeconds(0.1f);
        // TODO: tbf, idk if it works... I'd like to size the box to the text before I start printing text
        _lineBox.style.width = _lineLabel.resolvedStyle.width;
        _lineBox.style.height = _lineLabel.resolvedStyle.height;

        foreach (HeroCardMini c in _cardsInConversation)
        {
            if (c.Hero == line.Speaker.SpeakerHero)
            {
                if (!isSkipped) HandleSpeaker(c, line);
                UpdateBoxPosition(c);
            }
        }
    }

    void HandleSpeaker(HeroCardMini card, CutsceneLine line)
    {
        _currentSpeaker = card;
        float duration = 1f;
        if (line.VO != null)
            duration = line.VO.Clips[0].length;

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
            _lineBox.style.top = 20;
        else
            _lineBox.style.top = y - elHeight;
    }


    IEnumerator TypeText(CutsceneLine line)
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

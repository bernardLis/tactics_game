using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CutsceneManager : MonoBehaviour
{
    const string _ussClassName = "cutscene__";
    const string _ussBackground = _ussClassName + "background";
    const string _ussLineBox = _ussClassName + "line-box";
    const string _ussLineLabel = _ussClassName + "line-label";

    GameManager _gameManager;
    AudioManager _audioManager;
    DeskManager _deskManager;

    [SerializeField] Conversation _introConversation;
    [SerializeField] Character _banker;

    [SerializeField] Sound _snekSound;
    [SerializeField] Sprite[] _snekSprites;
    int snekIndex = 0;

    List<CharacterCardMini> _cardsInConversation = new();

    VisualElement _root;
    VisualElement _reportContainer;
    VisualElement _bg;
    VisualElement _lineBox;
    Label _lineLabel;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _audioManager = AudioManager.Instance;
        _deskManager = DeskManager.Instance;
        _deskManager.OnDeskInitialized += OnDeskInitialized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _reportContainer = _root.Q<VisualElement>("reportContainer");

        _lineBox = new();
        _lineBox.AddToClassList(_ussLineBox);
        _reportContainer.Add(_lineBox);

        _lineLabel = new();
        _lineLabel.AddToClassList(_ussLineLabel);
        _lineBox.Add(_lineLabel);
    }

    void OnDeskInitialized()
    {
        if (_gameManager.WasIntroCutscenePlayed)
            return;
        StartCoroutine(PlayIntroCutscene());
    }

    void OnDayPassed(int day)
    {
        if (day == 5)
            Debug.Log($"day 5 passed in cutscene manager");
    }

    IEnumerator PlayIntroCutscene()
    {


        _gameManager.ToggleTimer(false);
        yield return new WaitForSeconds(0.5f);
        _bg = new();
        _bg.AddToClassList(_ussBackground);
        _reportContainer.Add(_bg);

        List<CharacterCardMini> cards = _deskManager.GetAllCharacterCardsMini();
        Vector3 pos = new Vector3(150, 200);
        foreach (var c in cards)
        {
            _cardsInConversation.Add(c);
            c.BringToFront();
            c.style.left = pos.x;
            c.style.top = pos.y;
            pos += new Vector3(0, 200);
        }

        // spawn a banker portrait on the right of the desk
        _banker.InitializeSpecialCharacter();

        CharacterCardMini bankerCard = new(_banker);
        _cardsInConversation.Add(bankerCard);

        bankerCard.style.position = Position.Absolute;
        bankerCard.transform.position = new Vector3(Screen.width - 300, Screen.height * 0.3f);
        _reportContainer.Add(bankerCard);
        // display text from banker and you/friend
        yield return new WaitForSeconds(0.1f);

        foreach (ConversationLine line in _introConversation.Lines)
        {
            yield return HandleLineBox(line);
            yield return TypeText(line);
        }

        yield return null;


    }

    IEnumerator HandleLineBox(ConversationLine line)
    {
        _lineBox.style.visibility = Visibility.Hidden;
        _lineLabel.text = line.Text;

        yield return new WaitForSeconds(0.1f);
        Debug.Log($"width when the label is full: {_lineBox.resolvedStyle.width}");
        Debug.Log($"height when the label is full: {_lineBox.resolvedStyle.height}");

        _lineLabel.style.width = _lineLabel.resolvedStyle.width;
        _lineLabel.style.height = _lineLabel.resolvedStyle.height;

        if (line.Player)
        {
            UpdateBoxPosition(_cardsInConversation[0]);
            yield break;
        }

        if (line.Friend)
        {

            UpdateBoxPosition(_cardsInConversation[1]);
            yield break;
        }

        foreach (CharacterCardMini c in _cardsInConversation)
        {
            if (c.Character == line.SpeakerCharacter)
            {
                UpdateBoxPosition(c);
            }
        }

    }

    void UpdateBoxPosition(CharacterCardMini card)
    {
        _lineBox.BringToFront();
        // TODO: inelegant solution, when character card is tooltip element it does not resolve style for some reason. 
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
        if (snekIndex == 1)
            _audioManager.PlaySFX(_snekSound, Vector3.one);
        if (snekIndex >= 1)
        {
            for (int i = 0; i < 10; i++)
            {
                VisualElement container = new();
                container.style.width = 100;
                container.style.height = 100;

                AnimationElement el = new(_snekSprites, 300, true);
                el.PlayAnimation();
                container.Add(el);
                container.style.position = Position.Absolute;
                container.style.left = Random.Range(0, Screen.width);
                container.style.top = Random.Range(0, Screen.height);
                _bg.Add(container);
            }
        }

        _lineBox.style.visibility = Visibility.Visible;
        _lineLabel.text = "";

        char[] charArray = line.Text.ToCharArray();

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

        yield return new WaitForSeconds(0.5f);
        snekIndex++;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] Character Banker;
    GameManager _gameManager;
    DeskManager _deskManager;

    VisualElement _root;
    VisualElement _reportContainer;
    VisualElement _bg;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _deskManager = DeskManager.Instance;
        _deskManager.OnDeskInitialized += OnDeskInitialized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _reportContainer = _root.Q<VisualElement>("reportContainer");


        Debug.Log($"start cutscene manager ,was intro playued? {_gameManager.WasIntroCutscenePlayed}");
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
        Debug.Log($"Playing intro cutscene");
        _bg = new();
        _bg.style.width = Length.Percent(100);
        _bg.style.height = Length.Percent(100);
        _bg.style.position = Position.Absolute;
        _bg.style.backgroundColor = Color.black;
        _bg.style.opacity = 0.5f;

        _reportContainer.Add(_bg);

        List<CharacterCardMini> cards = _deskManager.GetAllCharacterCardsMini();
        Vector3 pos = new Vector3(150, 200);
        foreach (var c in cards)
        {
            Debug.Log($"pos: {pos}");
            c.BringToFront();
            c.style.left = pos.x;
            c.style.top = pos.y;
            pos += new Vector3(0, 200);
        }

        // spawn a banker portrait on the right of the desk
        Banker.InitializeSpecialCharacter();
        CharacterCardMini bankerCard = new(Banker);
        bankerCard.style.position = Position.Absolute;
        bankerCard.transform.position = new Vector3(Screen.width - 300, Screen.height * 0.3f);
        _reportContainer.Add(bankerCard);
        // display text from banker and you/friend

        yield return null;


    }
}

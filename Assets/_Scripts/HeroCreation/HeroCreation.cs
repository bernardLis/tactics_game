using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class HeroCreation : MonoBehaviour
{
    const string _ussCommonMenuButton = "common__menu-button";

    GameManager _gameManager;
    CutsceneManager _cutsceneManager;

    VisualElement _root;
    GameObject _starEffect;

    [SerializeField] Cutscene _nameCutscene;
    [SerializeField] Cutscene _looksCutscene;
    [SerializeField] Cutscene _elementCutscene;
    [SerializeField] Cutscene _endCutscene;

    VisualElement _background;

    VisualElement _wrapper;
    TextField _nameField;

    VisualElement _portraitContainer;
    Label _portraitTextLabel;
    VisualElement _portrait;
    VisualElement _portraitButtonContainer;

    VisualElement _elementChoiceContainer;

    MyButton _previousPortraitButton;
    MyButton _nextPortraitButton;

    VisualElement _submitButtonContainer;
    MyButton _submitButton;

    List<HeroPortrait> _heroPortraits = new();
    int _currentPortraitIndex = 0;

    Element _chosenElement;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;
    }

    // Start is called before the first frame update
    void Start()
    {
        _background = _root.Q<VisualElement>("background");

        _wrapper = _root.Q<VisualElement>("wrapper");
        _nameField = _root.Q<TextField>("nameField");

        _portraitContainer = _root.Q<VisualElement>("portraitGroupContainer");
        _portraitTextLabel = _root.Q<Label>("portraitTextLabel");
        _portrait = _root.Q<VisualElement>("portrait");
        _portraitButtonContainer = _root.Q<VisualElement>("portraitButtonContainer");

        _elementChoiceContainer = _root.Q<VisualElement>("elementChoiceContainer");

        _submitButtonContainer = _root.Q<VisualElement>("submitButtonContainer");

        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;
        _starEffect = _gameManager.GetComponent<EffectManager>()
                .PlayEffectWithName("TwinklingStarEffect", Vector3.zero, Vector3.one);


        // HERE: testing
        _chosenElement = _gameManager.HeroDatabase.GetRandomElement();
        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation("asd", _gameManager.HeroDatabase.GetRandomPortraitFemale(),
                 _chosenElement);
        _gameManager.PlayerHero = newChar;
        StartGame();
        /*

        _cutsceneManager = _gameManager.GetComponent<CutsceneManager>();
        _cutsceneManager.Initialize(_root);
        _cutsceneManager.OnCutsceneFinished += NameCutsceneFinished;

        NameFieldSetup();
        PortraitSetup();
        CreatePortraitButtons();

        StartCoroutine(StartShow());
        */


    }

    IEnumerator StartShow()
    {
        yield return new WaitForSeconds(0.5f);
        _cutsceneManager.PlayCutscene(_nameCutscene);
    }

    void NameCutsceneFinished(Cutscene c)
    {
        _cutsceneManager.OnCutsceneFinished -= NameCutsceneFinished;

        _nameField.style.display = DisplayStyle.Flex;
        _background.BringToFront();
        _submitButton = new("Submit", _ussCommonMenuButton, () =>
        {
            Hero newChar = ScriptableObject.CreateInstance<Hero>();
            newChar.HeroName = _nameField.value;
            _gameManager.PlayerHero = newChar;

            _nameField.style.display = DisplayStyle.None;
            _wrapper.Remove(_submitButton);
            _cutsceneManager.PlayCutscene(_looksCutscene);
            _cutsceneManager.OnCutsceneFinished += LooksCutsceneFinished;
        });
        _wrapper.Add(_submitButton);
    }

    void LooksCutsceneFinished(Cutscene c)
    {
        _cutsceneManager.OnCutsceneFinished -= LooksCutsceneFinished;

        _portraitContainer.style.display = DisplayStyle.Flex;
        _submitButton = new("Submit", _ussCommonMenuButton, () =>
        {
            _portraitContainer.style.display = DisplayStyle.None;
            _wrapper.Remove(_submitButton);
            _cutsceneManager.PlayCutscene(_elementCutscene);
            _cutsceneManager.OnCutsceneFinished += ElementCutsceneFinished;
        });
        _wrapper.Add(_submitButton);
    }

    void ElementCutsceneFinished(Cutscene c)
    {
        _cutsceneManager.OnCutsceneFinished -= ElementCutsceneFinished;
        _elementChoiceContainer.style.display = DisplayStyle.Flex;

        ElementChoiceElement elementChoiceElement = new();
        elementChoiceElement.OnElementChosen += ElementChosen;
        _elementChoiceContainer.Add(elementChoiceElement);
    }

    void ElementChosen(Element element)
    {
        _chosenElement = element;
        _gameManager.RivalHero.Element = element.WeakAgainst;
        _gameManager.RivalHero.Army = new(_gameManager.HeroDatabase.GetStartingArmy(element.WeakAgainst).Creatures);
        CreateHero();

        _elementChoiceContainer.style.display = DisplayStyle.None;
        _cutsceneManager.PlayCutscene(_endCutscene);
        _cutsceneManager.OnCutsceneFinished += EndHeroCreation;
    }

    void NameFieldSetup()
    {
        if (Random.value > 0.5f)
            _nameField.value = _gameManager.HeroDatabase.GetRandomNameFemale();
        else
            _nameField.value = _gameManager.HeroDatabase.GetRandomNameMale();
    }

    void PortraitSetup()
    {
        _heroPortraits = new(_gameManager.HeroDatabase.GetAllPortraits());
        _heroPortraits = _heroPortraits.OrderBy(x => Random.value).ToList();

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[0].Sprite);
    }

    void CreatePortraitButtons()
    {
        _previousPortraitButton = new("<-", _ussCommonMenuButton, PreviousPortrait);
        _nextPortraitButton = new("->", _ussCommonMenuButton, NextPortrait);

        _portraitButtonContainer.Add(_previousPortraitButton);
        _portraitButtonContainer.Add(_nextPortraitButton);
    }

    void PreviousPortrait()
    {
        _currentPortraitIndex--;
        if (_currentPortraitIndex < 0)
            _currentPortraitIndex = _heroPortraits.Count - 1;

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[_currentPortraitIndex].Sprite);
    }

    void NextPortrait()
    {
        _currentPortraitIndex++;
        if (_currentPortraitIndex >= _heroPortraits.Count)
            _currentPortraitIndex = 0;

        _portrait.style.backgroundImage = new StyleBackground(_heroPortraits[_currentPortraitIndex].Sprite);
    }

    void EndHeroCreation(Cutscene c)
    {
        _cutsceneManager.OnCutsceneFinished -= EndHeroCreation;
        StartGame();
        _gameManager.SaveJsonData();
    }

    void CreateHero()
    {
        Debug.Log($"Creating hero: {_nameField.value}");

        Hero newChar = ScriptableObject.CreateInstance<Hero>();
        newChar.CreateFromHeroCreation(_nameField.value, _heroPortraits[_currentPortraitIndex], _chosenElement);
        _gameManager.PlayerHero = newChar;
    }

    void StartGame()
    {
        // HERE: waves
        //        _gameManager.SelectedBattle.Opponent.Element = _chosenElement.StrongAgainst;
        //      _gameManager.SelectedBattle.Opponent.Army = new(_gameManager.HeroDatabase.GetStartingArmy(_chosenElement.StrongAgainst).Creatures);

        Debug.Log($"Starting game");
        _gameManager.StartGame();
    }
}

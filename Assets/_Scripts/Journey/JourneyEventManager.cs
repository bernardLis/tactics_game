using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class JourneyEventManager : MonoBehaviour
{
    GameManager _gameManager;
    AudioManager _audioManager;
    RunManager _runManager;

    VisualElement _root;

    Label _eventDescription;
    VisualElement _eventWrapper;
    VisualElement _optionsWrapper;

    VisualElement _responseWrapper;
    Label _responseLabel;
    VisualElement _rewardWrapper;
    MyButton _backToJourneyButton;

    public JourneyEvent _journeyEvent;

    List<MyButton> _optionButtons = new();

    ScreenWithDraggables _screenWithDraggables;

    List<EventOptionElement> _eventOptionElements = new();


    EventOptionElement _selectedOption;
    bool _wasWarned;


    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _runManager = RunManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _eventWrapper = _root.Q<VisualElement>("eventWrapper");
        _eventDescription = _root.Q<Label>("eventDescription");
        _optionsWrapper = _root.Q<VisualElement>("optionsWrapper");

        _responseWrapper = _root.Q<VisualElement>("responseWrapper");
        _responseLabel = _root.Q<Label>("response");
        _rewardWrapper = _root.Q<VisualElement>("rewardWrapper");

        _backToJourneyButton = new MyButton("Continue", "menuButton", BackToJourney);

        SetupEvent();
        // CreateOptions();
    }

    async void SetupEvent()
    {
        // HERE: _journeyEvent = _runManager.ChooseEvent();
        _audioManager.PlayDialogue(_journeyEvent.VoiceOver);

        _screenWithDraggables = new(_root);
        _screenWithDraggables.style.backgroundImage = _journeyEvent.Background.texture;
        _screenWithDraggables.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

        Label eventDescription = new Label(_journeyEvent.Description);
        eventDescription.AddToClassList("description");
        _screenWithDraggables.AddElement(eventDescription);

        int delay = Mathf.CeilToInt(_journeyEvent.VoiceOver.Clips[0].length * 1000);
        Debug.Log($"delay: {delay}");
        await Task.Delay(delay);
        Debug.Log("after delay");
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceAround;
        _screenWithDraggables.AddElement(container);

        foreach (EventOption option in _journeyEvent.Options)
        {
            EventOptionElement element = new EventOptionElement(option, _screenWithDraggables);
            element.style.visibility = Visibility.Hidden;
            _eventOptionElements.Add(element);
            container.Add(element);
            element.OnMouseEnter += OnMouseEnterOptionElement;
            element.OnMouseLeave += OnMouseLeaveOptionElement;
            element.OnPointerUp += OnPointerUpOptionElement;
        }

        foreach (EventOptionElement element in _eventOptionElements)
            await FadeIn(element);


        await FadeIn(_screenWithDraggables.AddPouches());
        await FadeIn(_screenWithDraggables.AddCharacters(_runManager.PlayerTroops));

        _backToJourneyButton.style.visibility = Visibility.Hidden;
        _screenWithDraggables.AddElement(_backToJourneyButton);
    }

    async Task FadeIn(VisualElement element)
    {
        element.style.visibility = Visibility.Visible;
        float o = 0;
        while (o < 1f)
        {
            element.style.opacity = o;
            o += 0.01f;
            await Task.Delay(5);
        }
    }

    void OnMouseEnterOptionElement(EventOptionElement activeOption)
    {
        foreach (EventOptionElement option in _eventOptionElements)
        {
            if (activeOption == option)
                continue;

            option.style.opacity = 0.5f;
        }
    }

    void OnMouseLeaveOptionElement()
    {
        foreach (EventOptionElement option in _eventOptionElements)
            option.style.opacity = 1f;

    }

    void OnPointerUpOptionElement(EventOptionElement activeOption)
    {
        FadeIn(_backToJourneyButton).GetAwaiter();

        _selectedOption = activeOption;
        _selectedOption.UnlockRewards();

        foreach (EventOptionElement option in _eventOptionElements)
        {
            option.UnregisterCallbacks();

            if (activeOption == option)
                continue;

            option.LockRewards();

        }

    }

    void BackToJourney()
    {
        if (!_selectedOption.WasRewardTaken() && !_wasWarned)
        {
            _wasWarned = true;
            return;
        }

        _runManager.VisitedJourneyNodes.Add(_runManager.CurrentNode.Serialize());
        _gameManager.LoadLevel(Scenes.Journey);
    }
}

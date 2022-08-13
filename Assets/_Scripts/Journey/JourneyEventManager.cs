using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class JourneyEventManager : MonoBehaviour
{
    GameManager _gameManager;
    AudioManager _audioManager;
    RunManager _runManager;

    Label _eventDescription;
    VisualElement _eventWrapper;
    VisualElement _optionsWrapper;

    VisualElement _responseWrapper;
    Label _responseLabel;
    VisualElement _rewardWrapper;
    Button _backToJourneyButton;

    JourneyEvent _journeyEvent;

    List<Button> _optionButtons = new();

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _runManager = RunManager.Instance;

        var root = GetComponent<UIDocument>().rootVisualElement;
        _eventWrapper = root.Q<VisualElement>("eventWrapper");
        _eventDescription = root.Q<Label>("eventDescription");
        _optionsWrapper = root.Q<VisualElement>("optionsWrapper");

        _responseWrapper = root.Q<VisualElement>("responseWrapper");
        _responseLabel = root.Q<Label>("response");

        _rewardWrapper = root.Q<VisualElement>("rewardWrapper");

        _backToJourneyButton = root.Q<Button>("backToJourney");
        _backToJourneyButton.clickable.clicked += BackToJourney;

        SetupEvent();
        CreateOptions();
    }

    void SetupEvent()
    {
        _journeyEvent = _runManager.ChooseEvent();

        _eventWrapper.style.backgroundImage = _journeyEvent.Background.texture;
        _eventDescription.text = _journeyEvent.Description;

        _audioManager.PlayDialogue(_journeyEvent.VoiceOver);
    }

    void CreateOptions()
    {
        _optionsWrapper.Clear();

        for (int i = 0; i < _journeyEvent.Options.Count; i++)
        {
            Button b = new Button();
            _optionsWrapper.Add(b);

            b.text = _journeyEvent.Options[i].Text;
            b.userData = i;
            b.clickable.clickedWithEventInfo += OptionChosen;
            b.AddToClassList("optionButton");
            _optionButtons.Add(b);
        }
    }

    void OptionChosen(EventBase _evt)
    {
        foreach (Button b in _optionButtons)
            b.SetEnabled(false);

        Button clickedButton = _evt.target as Button;
        clickedButton.style.backgroundColor = Color.black;
        int index = int.Parse(clickedButton.userData.ToString()); // TODO: dunno if a good idea 
        _runManager.SetNodeReward(_journeyEvent.Options[index].Reward);

        _responseLabel.text = _journeyEvent.Options[index].Response;

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Label txt = new Label("You get: ");
        container.Add(txt);
        if (_journeyEvent.Options[index].Reward.Gold != 0)
        {
            Label gold = new(_journeyEvent.Options[index].Reward.Gold.ToString() + "Gold"); // TODO: gold icon
            container.Add(gold);
        }
        if (_journeyEvent.Options[index].Reward.Item != null)
        {
            ItemVisual item = new(_journeyEvent.Options[index].Reward.Item);
            container.Add(item);
        }
        _rewardWrapper.Add(container);

        _audioManager.PlayDialogue(_journeyEvent.Options[index].ResponseVoiceOver);

        _responseWrapper.style.opacity = 0;
        _responseWrapper.style.display = DisplayStyle.Flex;
        DOTween.To(() => _responseWrapper.style.opacity.value, x => _responseWrapper.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine);
    }

    void BackToJourney()
    {
        _gameManager.LoadLevel(Scenes.Journey);
    }
}

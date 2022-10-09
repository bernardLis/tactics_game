using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

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
        //_responseWrapper.Add(_backToJourneyButton);

        SetupEvent();
        // CreateOptions();
    }

    void SetupEvent()
    {
        // HERE: _journeyEvent = _runManager.ChooseEvent();

        // HERE: Do all elements have to be in screen with draggables...
        _eventWrapper.style.backgroundImage = _journeyEvent.Background.texture;
        _eventDescription.text = _journeyEvent.Description;

        _audioManager.PlayDialogue(_journeyEvent.VoiceOver);

        _screenWithDraggables = new(_root);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceAround;
        _screenWithDraggables.AddElement(container);

        foreach (EventOption option in _journeyEvent.Options)
        {
            EventOptionElement element = new EventOptionElement(option, _screenWithDraggables);
            _eventOptionElements.Add(element);
            container.Add(element);
            element.OnMouseEnter += OnMouseEnterOptionElement;
            element.OnMouseLeave += OnMouseLeaveOptionElement;

        }

        _screenWithDraggables.AddPouches();
        _screenWithDraggables.AddCharacters(_runManager.PlayerTroops);
        _screenWithDraggables.AddElement(_backToJourneyButton);
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
    /*
        void CreateOptions()
        {
            _optionsWrapper.Clear();

            for (int i = 0; i < _journeyEvent.Options.Count; i++)
            {
                MyButton b = new(_journeyEvent.Options[i].Text, "optionButton", null);
                _optionsWrapper.Add(b);

                b.userData = i;
                b.clickable.clickedWithEventInfo += OptionChosen;
                _optionButtons.Add(b);
            }
        }

            void OptionChosen(EventBase _evt)
            {
                foreach (MyButton b in _optionButtons)
                    b.SetEnabled(false);

                MyButton clickedButton = _evt.target as MyButton;
                clickedButton.style.backgroundColor = Color.black;
                int index = int.Parse(clickedButton.userData.ToString()); // TODO: dunno if a good idea 
                _runManager.SetNodeReward(_journeyEvent.Options[index].Reward);

                _responseLabel.text = _journeyEvent.Options[index].Response;
                _audioManager.PlayDialogue(_journeyEvent.Options[index].ResponseVoiceOver);

                _responseWrapper.style.opacity = 0;
                _responseWrapper.style.display = DisplayStyle.Flex;
                DOTween.To(() => _responseWrapper.style.opacity.value, x => _responseWrapper.style.opacity = x, 1f, 1f)
                    .SetEase(Ease.InSine);

                if (_journeyEvent.Options[index].Reward == null)
                    return;

                VisualElement container = new();
                container.style.flexDirection = FlexDirection.Row;
                Label txt = new Label("Reward: ");
                container.Add(txt);



                //        RewardsContainer rewardsContainer = new();


                if (_journeyEvent.Options[index].Reward.Obols != 0)
                {
                    Label obols = new(_journeyEvent.Options[index].Reward.Obols.ToString() + "Obols"); // TODO: gold icon
                    container.Add(obols);
                }
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
                if (_journeyEvent.Options[index].Reward.Recruit != null)
                {
                    CharacterCardVisualExtended card = new(_journeyEvent.Options[index].Reward.Recruit);
                    container.Add(card);
                }


                _rewardWrapper.Add(container);
            }
            */

    void BackToJourney()
    {
        _runManager.VisitedJourneyNodes.Add(_runManager.CurrentNode.Serialize());
        _gameManager.LoadLevel(Scenes.Journey);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class JourneyEventManager : MonoBehaviour
{
    JourneyManager journeyManager;
    LevelLoader levelLoader;

    UIDocument UIDocument;
    Label eventDescription;
    VisualElement eventWrapper;
    VisualElement optionsWrapper;

    VisualElement responseWrapper;
    Label responseLabel;
    VisualElement rewardWrapper;
    Label obolAmountLabel;
    Button backToJourneyButton;

    JourneyEvent journeyEvent;

    List<Button> optionButtons = new();

    void Awake()
    {
        journeyManager = JourneyManager.instance;
        levelLoader = journeyManager.GetComponent<LevelLoader>();

        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        eventWrapper = root.Q<VisualElement>("eventWrapper");
        eventDescription = root.Q<Label>("eventDescription");
        optionsWrapper = root.Q<VisualElement>("optionsWrapper");

        responseWrapper = root.Q<VisualElement>("responseWrapper");
        responseLabel = root.Q<Label>("response");
        rewardWrapper = root.Q<VisualElement>("rewardWrapper");
        obolAmountLabel = root.Q<Label>("obolAmount");
        backToJourneyButton = root.Q<Button>("backToJourney");

        backToJourneyButton.clickable.clicked += BackToJourney;

        SetupEvent();
        CreateOptions();
    }

    void SetupEvent()
    {
        journeyEvent = journeyManager.ChooseEvent();

        eventWrapper.style.backgroundImage = journeyEvent.background.texture;
        eventDescription.text = journeyEvent.description;
    }

    void CreateOptions()
    {
        optionsWrapper.Clear();

        for (int i = 0; i < journeyEvent.options.Count; i++)
        {
            Button b = new Button();
            optionsWrapper.Add(b);

            b.text = journeyEvent.options[i].text + "(" + journeyEvent.options[i].reward.obols + ")";
            b.userData = i;
            b.clickable.clickedWithEventInfo += OptionChosen;
            b.AddToClassList("optionButton");
            optionButtons.Add(b);
        }
    }

    void OptionChosen(EventBase _evt)
    {
        foreach (Button b in optionButtons)
            b.SetEnabled(false);

        Button clickedButton = _evt.target as Button;
        clickedButton.style.backgroundColor = Color.black;
        int index = int.Parse(clickedButton.userData.ToString()); // TODO: dunno if a good idea 
        journeyManager.SetNodeReward(journeyEvent.options[index].reward);

        responseLabel.text = journeyEvent.options[index].response;
        obolAmountLabel.text = journeyEvent.options[index].reward.obols.ToString();

        responseWrapper.style.opacity = 0;
        responseWrapper.style.display = DisplayStyle.Flex;
        DOTween.To(() => responseWrapper.style.opacity.value, x => responseWrapper.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine);
    }


    void BackToJourney()
    {
        levelLoader.LoadLevel("Journey");
    }
}

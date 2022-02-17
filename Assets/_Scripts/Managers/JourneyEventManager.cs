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

    JourneyEvent journeyEvent;

    void Awake()
    {
        journeyManager = JourneyManager.instance;
        levelLoader = journeyManager.GetComponent<LevelLoader>();

        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        eventWrapper = root.Q<VisualElement>("eventWrapper");
        eventDescription = root.Q<Label>("eventDescription");
        optionsWrapper = root.Q<VisualElement>("optionsWrapper");

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
            b.clickable.clickedWithEventInfo += BackToJourney;
            b.AddToClassList("optionButton");

        }
    }

    void BackToJourney(EventBase _evt)
    {
        var b = _evt.target as Button;
        int index = int.Parse(b.userData.ToString()); // TODO: dunno if a good idea 
        journeyManager.SetNodeReward(journeyEvent.options[index].reward);
        levelLoader.ChangeScene("Journey");
    }
}

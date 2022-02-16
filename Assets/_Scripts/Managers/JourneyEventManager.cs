using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class JourneyEventManager : MonoBehaviour
{
    JourneyManager journeyManager;

    UIDocument UIDocument;
    Label eventDescription;
    VisualElement eventWrapper;
    VisualElement optionsWrapper;

    JourneyEvent journeyEvent;


    void Awake()
    {
        journeyManager = JourneyManager.instance;

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
        journeyEvent = journeyManager.currentJourneyNode.journeyEvent;

        eventWrapper.style.backgroundImage = journeyEvent.background.texture;
        eventDescription.text = journeyEvent.description;
    }


    void CreateOptions()
    {
        optionsWrapper.Clear();

        foreach (JourneyEventOption option in journeyEvent.options)
        {
            Button b = new Button();
            optionsWrapper.Add(b);

            b.text = option.text;
            b.clickable.clicked += BackToJourney;
            // TODO: I could be holding result in here and then acting on it in Journey scene.
        }
    }

    void BackToJourney()
    {
        Debug.Log("back to journey");
        SceneManager.LoadScene("Journey");
    }

}

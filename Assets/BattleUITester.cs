using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleUITester : MonoBehaviour
{
    JourneyManager _journeyManager;
    VisualElement _battleEndContainer;
    Label _battleEndText;
    VisualElement _battleEndCharacters;
    Button _backToJourney;

    void Start()
    {
        _journeyManager = JourneyManager.instance;

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _battleEndContainer = root.Q<VisualElement>("battleEndContainer");
        _battleEndText = root.Q<Label>("battleEndText");
        _battleEndCharacters = root.Q<VisualElement>("battleEndCharacters");
        _backToJourney = root.Q<Button>("backToJourney");
    }

    public void ShowBattleWonScreen()
    {
        ShowBattleEndScreen();
        foreach (Character character in _journeyManager.PlayerTroops)
        {
            CharacterCardVisual visual = new CharacterCardVisual(character);
            _battleEndCharacters.Add(visual);
        }

        _battleEndText.text = "WON!!";
    }

    void ShowBattleEndScreen()
    {
        _battleEndContainer.style.display = DisplayStyle.Flex;
        _battleEndContainer.style.opacity = 0f;
        DOTween.To(() => _battleEndContainer.style.opacity.value, x => _battleEndContainer.style.opacity = x, 1f, 2f)
            .OnComplete(() => _backToJourney.style.display = DisplayStyle.Flex);
    }

}

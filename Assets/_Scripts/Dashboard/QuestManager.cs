using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableCharacters _draggableCharacters;
    VisualElement _root;

    ScrollView _questsList;
    VisualElement _questTroopsContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;
        _dashboardManager.OnQuestsClicked += Initialize;

        _draggableCharacters = GetComponent<DraggableCharacters>();

        _questsList = _root.Q<ScrollView>("questList");
        _questTroopsContainer = _root.Q<VisualElement>("questTroopsContainer");

        Initialize();
    }

    void Initialize()
    {
        _questsList.Clear();
        _questTroopsContainer.Clear();

        foreach (Quest q in _gameManager.AvailableQuests)
            _questsList.Add(new QuestVisualElement(q));

        foreach (Character character in _gameManager.PlayerTroops)
            if (!character.IsUnavailable)
                _questTroopsContainer.Add(new CharacterCardMiniSlot(new CharacterCardMini(character)));

        _questTroopsContainer.Add(new CharacterCardMiniSlot());

        _draggableCharacters.Initialize(_root);
    }

}

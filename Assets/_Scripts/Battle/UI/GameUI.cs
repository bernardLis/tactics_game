using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : Singleton<GameUI>
{
    // UI
    VisualElement _root;

    protected override void Awake()
    {
        base.Awake();
        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;
    }

    public void HideAllUIPanels()
    {
        CharacterUI.Instance.HideCharacterUI();
        InfoCardUI.Instance.HideCharacterCard();
        InfoCardUI.Instance.HideInteractionSummary();
        InfoCardUI.Instance.HideTileInfo();

        _root.Q<VisualElement>("inventoryContainer").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("questContainer").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("conversationContainer").style.display = DisplayStyle.None;
    }
}

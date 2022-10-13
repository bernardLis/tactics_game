using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class Armory : MonoBehaviour
{
    Dashboard _dashboard;
    UIDraggables _uiDraggables;
    VisualElement _root;

    VisualElement _armoryCharacters;
    VisualElement _armoryItems;
    VisualElement _armoryAbilities;

    // Start is called before the first frame update
    void Start()
    {
        _dashboard = GetComponent<Dashboard>();
        _uiDraggables = GetComponent<UIDraggables>();

        _root = _dashboard.Root;

        _armoryCharacters = _root.Q<VisualElement>("armoryCharacters");
        _armoryItems = _root.Q<VisualElement>("armoryItems");
        _armoryAbilities = _root.Q<VisualElement>("armoryAbilities");

        _dashboard.OnArmoryClicked += InitializeArmory;

        _dashboard.OnAbilitiesClicked += CleanArmory;
        _dashboard.OnQuestsClicked += CleanArmory;
        _dashboard.OnShopClicked += CleanArmory;
    }

    void InitializeArmory()
    {
        _uiDraggables.Initialize(_root);
        
        _armoryCharacters.Add(_uiDraggables.CreateCharacterCards(RunManager.Instance.PlayerTroops));
        _armoryItems.Add(_uiDraggables.CreateItemPouch());
        _armoryAbilities.Add(_uiDraggables.CreateAbilityPouch());
    }

    void CleanArmory()
    {
        _uiDraggables.ClearDraggables();

        _armoryCharacters.Clear();
        _armoryItems.Clear();
        _armoryAbilities.Clear();
    }
}

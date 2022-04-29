using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class CharacterScreen : VisualElement
{
    VisualElement _root;
    public CharacterScreen(Character character, VisualElement root)
    {
        style.backgroundColor = Color.gray;
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);

        AddCharacterCard(character);
        AddAbilityContainer(character);

        _root = root;
        root.Add(this);

        RegisterCallback<PointerDownEvent>(OnPointerDown);
        StyleSheet ss = (StyleSheet)AssetDatabase.LoadAssetAtPath("Assets/UI/UIStyles/BattleUIStyles.uss", typeof(StyleSheet));
        styleSheets.Add(ss);
    }

    void AddCharacterCard(Character character)
    {
        VisualElement characterCardContainer = new();
        characterCardContainer.AddToClassList("battleUIContainer");
        CharacterCardVisual card = new CharacterCardVisual(character, false);

        characterCardContainer.Add(card);
        Add(characterCardContainer);
    }

    void AddAbilityContainer(Character character)
    {
        // add a container for abilities;
        VisualElement abilityContainer = new();
        abilityContainer.AddToClassList("battleUIContainer");
        //abilityContainer.style.width = Length.Percent(40);

        abilityContainer.style.flexDirection = FlexDirection.Row;
        Add(abilityContainer);

        List<Ability> allAbilities = new(character.BasicAbilities);
        allAbilities.AddRange(character.Abilities);

        foreach (Ability a in allAbilities)
        {
            VisualElement aContainer = new();
            aContainer.style.width = Length.Percent(20);
            aContainer.AddToClassList("battleUIContainer");
            aContainer.style.alignItems = Align.Center;
            aContainer.style.alignSelf = Align.FlexStart;

            AbilityButton button = new(a, null);
            aContainer.Add(button);
            AbilityTooltipVisual tooltip = new(a);
            aContainer.Add(tooltip);

            abilityContainer.Add(aContainer);
        }
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 1) // only right mouse click
            return;
        Debug.Log("click on characrter screen");
        Hide();
    }

    public void Hide()
    {
        _root.Remove(this);
    }
}

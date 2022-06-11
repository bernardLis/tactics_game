using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterScreen : FullScreenVisual
{

    // TODO: many repetitions in the next 4 functions
    public CharacterScreen(CharacterStats stats, VisualElement root)
    {
        BaseCharacterScreen(root);

        AddCharacterCard(stats);
        AddAbilityContainer(stats.Character);
        AddBackButton();
    }

    public CharacterScreen(Character character, VisualElement root)
    {
        BaseCharacterScreen(root);

        AddCharacterCard(character);
        AddAbilityContainer(character);
        AddBackButton();
    }

    void BaseCharacterScreen(VisualElement root)
    {
        Initialize(root);
        style.backgroundColor = Color.gray;
    }

    void AddCharacterCard(CharacterStats stats)
    {
        VisualElement characterCardContainer = new();
        characterCardContainer.AddToClassList("uiContainer");
        CharacterCardVisual card = new CharacterCardVisual(stats, false);

        characterCardContainer.Add(card);
        Add(characterCardContainer);
    }

    void AddCharacterCard(Character character)
    {
        VisualElement characterCardContainer = new();
        characterCardContainer.AddToClassList("uiContainer");
        CharacterCardVisual card = new CharacterCardVisual(character, false);

        characterCardContainer.Add(card);
        Add(characterCardContainer);
    }

    void AddAbilityContainer(Character character)
    {
        // add a container for abilities;
        VisualElement abilityContainer = new();
        abilityContainer.AddToClassList("uiContainer");

        abilityContainer.style.flexDirection = FlexDirection.Row;
        Add(abilityContainer);

        List<Ability> allAbilities = new(character.BasicAbilities);
        allAbilities.AddRange(character.Abilities);

        foreach (Ability a in allAbilities)
        {
            VisualElement aContainer = new();
            aContainer.style.width = Length.Percent(20);
            aContainer.AddToClassList("uiContainer");
            aContainer.style.alignItems = Align.Center;
            aContainer.style.alignSelf = Align.Auto;

            AbilityButton button = new(a, null);
            aContainer.Add(button);
            AbilityTooltipVisual tooltip = new(a);
            aContainer.Add(tooltip);

            abilityContainer.Add(aContainer);
        }
    }
}

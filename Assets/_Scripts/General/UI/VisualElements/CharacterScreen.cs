using System.Collections.Generic;
using UnityEngine.UIElements;

public class CharacterScreen : FullScreenVisual
{
    public CharacterScreen(Character character, VisualElement root)
    {
        Initialize(root);
        AddCharacterCard(character);
        AddAbilityContainer(character);
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
            aContainer.style.alignSelf = Align.FlexStart;

            AbilityButton button = new(a, null);
            aContainer.Add(button);
            AbilityTooltipVisual tooltip = new(a);
            aContainer.Add(tooltip);

            abilityContainer.Add(aContainer);
        }
    }
}

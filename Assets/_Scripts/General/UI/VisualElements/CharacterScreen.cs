using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterScreen : VisualElement
{
    VisualElement _root;
    public CharacterScreen(Character character, VisualElement root)
    {

        Debug.Log("creating a screen");
        style.backgroundColor = Color.gray;
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);

        VisualElement characterCardContainer = new();
        characterCardContainer.AddToClassList("battleUIContainer");
        Add(characterCardContainer);
        CharacterCardVisual card = new CharacterCardVisual(character);
        characterCardContainer.Add(card);

        // add a container for abilities;
        VisualElement abilityContainer = new();
        abilityContainer.AddToClassList("battleUIContainer");
        abilityContainer.style.flexDirection = FlexDirection.Row;
        Add(abilityContainer);

        // TODO: show basic abilities?
        foreach (Ability a in character.Abilities)
        {
            AbilityButton button = new(a, null);
            abilityContainer.Add(button);
        }

        // TODO: show ability tooltips somewhere?

        _root = root;
        root.Add(this);
    }

    public void Hide()
    {
        Debug.Log("removing this");

        /*
        if (this == null)
            return;
        if (this.parent == null)
            return;
        */
        _root.Remove(this);
    }

}

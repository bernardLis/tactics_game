using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroMeetingElement : FullScreenElement
{
    public HeroMeetingElement(VisualElement root, DraggableArmies draggableArmies, MapHero hero, MapHero otherHero)
    {
        Initialize(root);
        style.backgroundColor = new Color(0, 0, 0, 0.5f);
        style.alignItems = Align.Center;

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.height = Length.Percent(90);

        Debug.Log($"Hero meeting element not implemented");
        //HeroCardFull card = new(hero.Hero, root, draggableArmies, false);
        // HeroCardFull otherCard = new(otherHero.Hero, root, draggableArmies, false);
        //  container.Add(card);
        //  container.Add(otherCard);
        Add(container);

        AddBackButton();
        draggableArmies.Initialize();
    }
}

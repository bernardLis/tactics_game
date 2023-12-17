using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Tablet Advanced")]
public class TabletAdvanced : Tablet
{
    [Header("Advanced Tablet")]
    public Element FirstElement;
    public Element SecondElement;

    public Ability FirstAbility;
    public Ability SecondAbility;

    public override void Initialize(Hero hero)
    {
        base.Initialize(hero);

        // HERE: fixed by adding normal abilities
        FirstAbility.Element = Element;
        SecondAbility.Element = Element;

        FirstAbility.InitializeBattle(hero);
        SecondAbility.InitializeBattle(hero);
    }

    public override void LevelUp()
    {
        base.LevelUp();

        // HERE: something more festive on tablet max level and adding a second ability
        if (!IsMaxLevel()) return;

        if (_hero.Abilities.Contains(FirstAbility))
            _hero.AddAbility(SecondAbility);
        else
            _hero.AddAbility(FirstAbility);
    }

    public bool IsMadeOfElements(Element firstElement, Element secondElement)
    {
        Debug.Log($"is {name} made of {firstElement.name} and {secondElement.name}?");
        return (FirstElement == firstElement && SecondElement == secondElement) ||
               (FirstElement == secondElement && SecondElement == firstElement);
    }
}

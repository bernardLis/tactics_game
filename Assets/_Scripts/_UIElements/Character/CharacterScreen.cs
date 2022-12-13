using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterScreen : FullScreenElement
{
    // TODO: many repetitions in the next 4 functions
    public CharacterScreen(CharacterStats stats, VisualElement root)
    {
        BaseCharacterScreen(root);

        AddCharacterCard(stats);
        AddBackButton();
    }

    public CharacterScreen(Character character, VisualElement root)
    {
        BaseCharacterScreen(root);

        AddCharacterCard(character);
        AddBackButton();
    }

    void BaseCharacterScreen(VisualElement root)
    {
        Initialize(root);
        style.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;
    }

    void AddCharacterCard(CharacterStats stats)
    {
        CharacterCard card = new CharacterCard(stats.Character);
        Add(card);
    }

    void AddCharacterCard(Character character)
    {
        CharacterCard card = new CharacterCard(character);
        Add(card);
    }
}

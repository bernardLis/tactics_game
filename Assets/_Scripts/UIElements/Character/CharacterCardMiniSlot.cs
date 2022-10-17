using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardMiniSlot : VisualElementWithSound
{
    public CharacterCardMini Card;


    public event Action<CharacterCardMini> OnCardAdded;
    public event Action OnCardRemoved;

    public CharacterCardMiniSlot(CharacterCardMini card = null) : base()
    {
        AddToClassList("characterCardMiniSlot");
        if (card != null)
        {
            Card = card;
            AddCard(card);
        }
    }

    public void AddCard(CharacterCardMini card)
    {
        Card = card;
        Add(card);

        PlayClick();

        OnCardAdded?.Invoke(card);
    }

    public void RemoveCard()
    {
        Clear();
        Card = null;

        OnCardRemoved?.Invoke();
    }
}

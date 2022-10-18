using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardMiniSlot : VisualElementWithSound
{
    public CharacterCardMini Card;


    public event Action<CharacterCardMini> OnCardAdded;
    public event Action<CharacterCardMini> OnCardRemoved;

    public event Action<CharacterCardMiniSlot> OnLocked;

    public CharacterCardMiniSlot(CharacterCardMini card = null, bool isLocked = false) : base()
    {
        AddToClassList("characterCardMiniSlot");
        if (card != null)
        {
            Card = card;
            AddCard(card);
        }

        if (isLocked)
            Lock();
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
        OnCardRemoved?.Invoke(Card);
        Card = null;
    }

    public void Lock()
    {
        OnLocked?.Invoke(this);
        if (Card != null)
            Card.Lock();

        RemoveFromClassList("characterCardMiniSlot");
        AddToClassList("characterCardMiniSlotLocked");
    }

}

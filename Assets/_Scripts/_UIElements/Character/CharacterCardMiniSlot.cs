using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardMiniSlot : ElementWithSound
{
    public CharacterCardMini Card;

    public bool IsLocked { get; private set; }

    public event Action<CharacterCardMini> OnCardAdded;
    public event Action<CharacterCardMini> OnCardRemoved;

    public event Action<CharacterCardMiniSlot> OnLocked;

    public CharacterCardMiniSlot(CharacterCardMini card = null, bool isLocked = false) : base()
    {
        AddToClassList("characterCardMiniSlot");
        if (card != null)
            AddCard(card);

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
        CharacterCardMini previousCard = Card;
        Card = null;
        OnCardRemoved?.Invoke(previousCard);
    }

    public void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
        if (Card != null)
            Card.Lock();

        RemoveFromClassList("characterCardMiniSlot");
        AddToClassList("characterCardMiniSlotLocked");
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardMiniSlot : ElementWithSound
{
    GameManager _gameManager;

    public CharacterCardMini Card;

    public bool IsLocked { get; private set; }

    public event Action<CharacterCardMini> OnCardAdded;
    public event Action<CharacterCardMini> OnCardRemoved;

    public event Action<CharacterCardMiniSlot> OnLocked;

    const string ussClassName = "character-card-mini-slot";
    const string ussMain = ussClassName + "__main";
    const string ussLocked = ussClassName + "__locked";


    public CharacterCardMiniSlot(CharacterCardMini card = null, bool isLocked = false) : base()
    {
        _gameManager = GameManager.Instance;
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardMiniSlot);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(ussMain);
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

        RemoveFromClassList(ussMain);
        AddToClassList(ussLocked);
    }

}

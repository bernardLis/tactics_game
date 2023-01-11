using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class CharacterCardMiniSlot : ElementWithSound
{
    GameManager _gameManager;

    public CharacterCardMini Card;

    public bool IsLocked { get; private set; }
    bool _hasCard;

    public event Action<CharacterCardMini> OnCardAdded;
    public event Action<CharacterCardMini> OnCardRemoved;

    public event Action<CharacterCardMiniSlot> OnLocked;

    const string _ussClassName = "character-card-mini-slot";
    const string _ussMain = _ussClassName + "__main";
    const string _ussLocked = _ussClassName + "__locked";

    public CharacterCardMiniSlot(CharacterCardMini card = null, bool isLocked = false) : base()
    {
        _gameManager = GameManager.Instance;
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardMiniSlotStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        if (card != null)
            AddCard(card);

        if (isLocked)
            Lock();
    }

    public bool IsSlotEmpty() { return !_hasCard; }

    public void AddCard(CharacterCardMini card)
    {
        Card = card;
        _hasCard = true;
        card.Slotted();
        Add(card);

        PlayClick();

        OnCardAdded?.Invoke(card);
    }

    public void RemoveCard()
    {
        Card.Unslotted();
        OnCardRemoved?.Invoke(Card);
        _hasCard = false;
        Clear();
    }

    public void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
        if (Card != null && _hasCard)
            Card.Lock();

        RemoveFromClassList(_ussMain);
        AddToClassList(_ussLocked);
    }
}

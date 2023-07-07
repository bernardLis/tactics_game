using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleCard : ElementWithSound
{
    const string _ussClassName = "battle-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussDuelIcon = _ussClassName + "duel-icon";
    const string _ussWavesIcon = _ussClassName + "waves-icon";
    const string _ussDisabled = _ussClassName + "disabled";

    GameManager _gameManager;

    public Battle Battle { get; private set; }

    Label _battleTypeLabel;

    public event Action<BattleCard> OnCardSelected;
    public BattleCard(BattleType battleType)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        _battleTypeLabel = new Label();
        Add(_battleTypeLabel);

        Battle = ScriptableObject.CreateInstance<Battle>();
        Battle.OnBattleModifierAdded += OnBattleModifierAdded;

        if (battleType == BattleType.Duel)
            CreateDuel();
        if (battleType == BattleType.Waves)
            CreateWaves();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnBattleModifierAdded(BattleModifier battleModifier)
    {
        BattleModifierElement battleModifierElement = new(battleModifier, true);
        Add(battleModifierElement);
    }

    void CreateDuel()
    {
        _battleTypeLabel.text = "Duel";
        Battle.CreateRandomDuel(_gameManager.PlayerHero.Level.Value);

        HeroCardMini heroCardMini = new(Battle.Opponent);
        heroCardMini.SmallCard();
        Add(heroCardMini);
    }

    void CreateWaves()
    {
        _battleTypeLabel.text = "Waves";

        Battle.CreateRandomWaves(_gameManager.PlayerHero.Level.Value);

        HeroCardMini heroCardMini = new(Battle.Opponent);
        heroCardMini.SmallCard();
        Add(heroCardMini);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;

        _gameManager.SelectedBattle = Battle;
        OnCardSelected?.Invoke(this);

        _gameManager.GetComponent<AudioManager>().PlayUI("Hero Voices");
    }

    public void DisableCard()
    {
        SetEnabled(false);
        AddToClassList(_ussDisabled);
    }

    public void DisableClicks() { UnregisterCallback<PointerUpEvent>(OnPointerUp); }

}

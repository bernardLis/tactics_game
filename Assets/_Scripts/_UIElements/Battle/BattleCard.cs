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

    Battle _battle;

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

        _battle = ScriptableObject.CreateInstance<Battle>();

        if (battleType == BattleType.Duel)
            CreateDuel();
        if (battleType == BattleType.Waves)
            CreateWaves();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void CreateDuel()
    {
        _battleTypeLabel.AddToClassList(_ussDuelIcon);
        _battle.CreateRandomDuel(_gameManager.PlayerHero.Level.Value);

        HeroCardMini heroCardMini = new(_battle.Opponent);
        heroCardMini.SmallCard();
        Add(heroCardMini);

        VisualElement armyContainer = new();
        armyContainer.style.width = Length.Percent(80);
        armyContainer.style.flexDirection = FlexDirection.Row;
        armyContainer.style.flexWrap = Wrap.Wrap;
        Add(armyContainer);
        foreach (Creature c in _battle.Opponent.Army)
        {
            CreatureIcon creatureIcon = new(c);
            creatureIcon.SmallIcon();
            armyContainer.Add(creatureIcon);
        }
    }

    void CreateWaves()
    {
        _battleTypeLabel.AddToClassList(_ussWavesIcon);
        _battle.CreateRandomWaves(_gameManager.PlayerHero.Level.Value);

        HeroCardMini heroCardMini = new(_battle.Opponent);
        heroCardMini.SmallCard();
        Add(heroCardMini);

        Label waveCount = new Label("Number of waves: " + _battle.Waves.Count);
        Add(waveCount);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;

        _gameManager.SelectedBattle = _battle;
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

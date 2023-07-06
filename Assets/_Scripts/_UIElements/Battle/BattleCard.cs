using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleCard : ElementWithSound
{
    const string _ussClassName = "battle-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussDisabled = _ussClassName + "disabled";

    GameManager _gameManager;

    Battle _battle;

    public event Action<BattleCard> OnCardSelected;
    public BattleCard(BattleType battleType)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        _battle = ScriptableObject.CreateInstance<Battle>();

        if (battleType == BattleType.Duel)
            CreateDuel();
        if (battleType == BattleType.Waves)
            CreateWaves();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void CreateDuel()
    {
        _battle.CreateRandomDuel(_gameManager.PlayerHero.Level.Value);

        HeroCardMini heroCardMini = new(_battle.Opponent);
        Add(heroCardMini);

        ScrollView scrollView = new ScrollView();
        Add(scrollView);
        foreach (Creature c in _battle.Opponent.Army)
            scrollView.Add(new CreatureIcon(c));
    }

    void CreateWaves()
    {
        _battle.CreateRandomWaves(_gameManager.PlayerHero.Level.Value);

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

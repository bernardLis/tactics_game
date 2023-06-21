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
    public BattleCard()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        Hero opp = ScriptableObject.CreateInstance<Hero>();
        opp.CreateRandom(_gameManager.PlayerHero.Level.Value);

        if (_gameManager.BattleNumber == 1)
        {
            opp.Army.Clear();
            // get starting army of neutral element
            List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
            elements.Remove(_gameManager.PlayerHero.Element);
            elements.Remove(_gameManager.PlayerHero.Element.StrongAgainst);
            elements.Remove(_gameManager.PlayerHero.Element.WeakAgainst);
            opp.Army = new(_gameManager.HeroDatabase.GetStartingArmy(elements[0]).Creatures);
        }
        if (_gameManager.BattleNumber == 2)
        {
            // get starting army of element our here is weak to
            opp.Army = new(_gameManager.HeroDatabase.GetStartingArmy(_gameManager.PlayerHero.Element.WeakAgainst).Creatures);
        }

        _battle = ScriptableObject.CreateInstance<Battle>();
        _battle.Opponent = opp;

        HeroCardMini heroCardMini = new(opp);
        Add(heroCardMini);

        ScrollView scrollView = new ScrollView();
        Add(scrollView);
        foreach (Creature c in opp.Army)
            scrollView.Add(new CreatureIcon(c));

        RegisterCallback<PointerUpEvent>(OnPointerUp);
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

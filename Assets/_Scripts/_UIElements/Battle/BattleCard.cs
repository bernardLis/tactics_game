using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleCard : VisualElement
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

        Hero hero = ScriptableObject.CreateInstance<Hero>();
        hero.CreateRandom(_gameManager.PlayerHero.Level.Value);

        _battle = ScriptableObject.CreateInstance<Battle>();
        _battle.Opponent = hero;

        Add(new HeroCardMini(hero));

        ScrollView scrollView = new ScrollView();
        Add(scrollView);
        foreach (ArmyGroup ag in hero.Army)
            scrollView.Add(new ArmyGroupElement(ag));

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;

        _gameManager.SelectedBattle = _battle;
        OnCardSelected?.Invoke(this);
    }

    public void DisableCard() { AddToClassList(_ussDisabled); }

    public void DisableClicks() { UnregisterCallback<PointerUpEvent>(OnPointerUp); }

}

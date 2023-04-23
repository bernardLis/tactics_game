using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleCard : VisualElement
{
    GameManager _gameManager;

    Battle _battle;

    public event Action<BattleCard> OnCardSelected;
    public BattleCard()
    {
        _gameManager = GameManager.Instance;

        style.backgroundColor = Color.gray;

        Label l = new("Battle Card");
        Add(l);

        Hero hero = ScriptableObject.CreateInstance<Hero>();
        hero.CreateRandom(_gameManager.PlayerHero.Level.Value);

        _battle = ScriptableObject.CreateInstance<Battle>();
        _battle.Opponent = hero;

        Add(new HeroCardMini(hero));
        foreach (ArmyGroup ag in hero.Army)
        {
            Add(new ArmyElement(ag));
        }

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        _gameManager.SelectedBattle = _battle;
        OnCardSelected?.Invoke(this);
    }

    public void DisableCard()
    {
        transform.scale = Vector3.one * 0.8f;
        style.opacity = 0.6f;
    }

    public void DisableClicks()
    {
        UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

}

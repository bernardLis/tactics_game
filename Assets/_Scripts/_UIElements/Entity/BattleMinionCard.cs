using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleMinionCard : MinionCard
{
    const string _ussClassName = "battle-minion__";
    const string _ussMain = _ussClassName + "main";
    const string _ussKilledOverlay = _ussClassName + "killed-overlay";

    BattleMinion _battleMinion;

    Label _damageTaken;

    public BattleMinionCard(BattleMinion bm) : base(bm.Minion)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleMinionCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleMinion = bm;

        AddToClassList(_ussMain);

        _healthLabel.text = $"Health: {_battleMinion.CurrentHealth} / {_minion.GetHealth()}";

        _damageTaken = new($"Damage taken: {_battleMinion.DamageTaken}");

        _rightContainer.Clear();

        _rightContainer.Add(_damageTaken);

        _battleMinion.OnDamageTaken += OnDamageTaken;
        _battleMinion.OnDeath += OnDeath;
    }

    void OnHealthChanged(float nvm)
    {
        _healthLabel.text = $"Health: {_battleMinion.CurrentHealth} / {_minion.GetHealth()}";
    }

    void OnDamageTaken(int dmg)
    {
        _damageTaken.text = $"Damage taken: {_battleMinion.DamageTaken}";
    }

    void OnDeath(BattleEntity entity, BattleEntity killer, Ability ability)
    {
        VisualElement overlay = new();
        Label l = new Label("Defeated!");
        l.style.fontSize = 36;
        l.style.color = Color.white;
        overlay.Add(l);
        overlay.AddToClassList(_ussKilledOverlay);
        Add(overlay);
    }

}

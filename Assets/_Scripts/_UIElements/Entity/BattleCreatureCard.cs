using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleCreatureCard : CreatureCard
{
    const string _ussClassName = "battle-creature__";
    const string _ussMain = _ussClassName + "main";
    const string _ussKilledOverlay = _ussClassName + "killed-overlay";

    BattleCreature _battleCreature;

    Label _kills;
    Label _damageDealt;
    Label _damageTaken;

    public BattleCreatureCard(BattleCreature battleCreature) : base(battleCreature.Creature)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleCreatureCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleCreature = battleCreature;

        AddToClassList(_ussMain);

        if (_battleCreature.Entity.Hero != null)
            _power.text += " + " + Mathf.RoundToInt(_battleCreature.Entity.Hero.Power.GetValue());

        _healthLabel.text = $"Health: {_battleCreature.CurrentHealth} / {_creature.GetHealth()}";

        _kills = new($"Killed enemies: {_battleCreature.KilledEnemiesCount}");
        _damageDealt = new($"Damage dealt: {_battleCreature.DamageDealt}");
        _damageTaken = new($"Damage taken: {_battleCreature.DamageTaken}");

        _rightContainer.Clear();

        _rightContainer.Add(_kills);
        _rightContainer.Add(_damageDealt);
        _rightContainer.Add(_damageTaken);

        _rightContainer.Add(new CreatureAbilityElement(battleCreature.Creature.CreatureAbility,
                battleCreature.CurrentSpecialAbilityCooldown));

        _battleCreature.OnHealthChanged += OnHealthChanged;
        _battleCreature.OnEnemyKilled += OnEnemyKilled;
        _battleCreature.OnDamageDealt += OnDamageDealt;
        _battleCreature.OnDamageTaken += OnDamageTaken;
        _battleCreature.OnDeath += OnDeath;
    }

    void OnHealthChanged(float nvm)
    {
        _healthLabel.text = $"Health: {_battleCreature.CurrentHealth} / {_creature.GetHealth()}";
    }

    void OnEnemyKilled(int total)
    {
        _kills.text = $"Killed enemies: {total}";
    }

    void OnDamageDealt(int dmg)
    {
        _damageDealt.text = $"Damage dealt: {_battleCreature.DamageDealt}";
    }

    void OnDamageTaken(int dmg)
    {
        _damageTaken.text = $"Damage taken: {_battleCreature.DamageTaken}";
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

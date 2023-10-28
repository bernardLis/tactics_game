using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Cinemachine;

public class BattleHero : BattleEntity
{
    public Hero Hero { get; private set; }

    BattleFightManager _battleFightManager;

    BattleHeroController _thirdPersonController;
    BattleHeroHealthBar _battleHeroHealthBar;

    Dictionary<Ability, GameObject> _battleAbilities = new();

    public override void InitializeEntity(Entity entity, int team)
    {
        base.InitializeEntity(entity, 0);
        _agent.enabled = true;

        Hero = (Hero)entity;

        _thirdPersonController = GetComponent<BattleHeroController>();
        _thirdPersonController.SetMoveSpeed(Hero.Speed.GetValue());
        Hero.Speed.OnValueChanged += _thirdPersonController.SetMoveSpeed;

        _battleHeroHealthBar = GetComponentInChildren<BattleHeroHealthBar>();
        _battleHeroHealthBar.Initialize(Hero);

        Animator.enabled = true;

        HandleAbilities();
    }

    void HandleAbilities()
    {
        Hero.OnAbilityAdded += AddAbility;
        Hero.OnAbilityRemoved += RemoveAbility;

        _battleFightManager = BattleFightManager.Instance;
        _battleFightManager.OnFightStarted += OnFightStarted;
        _battleFightManager.OnFightEnded += OnFightEnded;

        foreach (Ability a in Hero.Abilities)
            AddAbility(a);

    }

    void OnFightStarted()
    {
        foreach (GameObject g in _battleAbilities.Values)
            g.GetComponent<BattleAbility>().StartAbility();
    }

    void OnFightEnded()
    {
        foreach (GameObject g in _battleAbilities.Values)
            g.GetComponent<BattleAbility>().StopAbility();
    }

    void OnDestroy()
    {
        Hero.OnAbilityAdded -= AddAbility;
        Hero.OnAbilityRemoved -= RemoveAbility;
    }

    void AddAbility(Ability ability)
    {
        GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab);
        abilityPrefab.transform.SetParent(transform);
        abilityPrefab.GetComponent<BattleAbility>().Initialize(ability,
                                                        _battleFightManager.IsFightActive);
        _battleAbilities.Add(ability, abilityPrefab);
    }

    void RemoveAbility(Ability ability)
    {
        // TODO: idk if it works...
        Destroy(_battleAbilities[ability]);
        _battleAbilities.Remove(ability);
    }

    public override void Engage(BattleEntity engager) { }

    public void GetHit(BattleEntity entity)
    {
        Hero.CurrentHealth.ApplyChange(-5);
        DisplayFloatingText($"{-5}", _gameManager.GameDatabase.GetColorByName("Health").Color);

        if (Hero.CurrentHealth.Value <= 0)
        {
            Die();
            return;
        }
    }

    public override IEnumerator GetHit(EntityFight attacker, int specialDamage = 0)
    {
        Hero.CurrentHealth.ApplyChange(-5);
        DisplayFloatingText($"{-5}", _gameManager.GameDatabase.GetColorByName("Health").Color);

        if (Hero.CurrentHealth.Value <= 0)
        {
            Die();
            yield break;
        }
        yield return null;
    }

    void Die()
    {
        Debug.Log($"Hero dies...");
    }
}

using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class CharacterStats : MonoBehaviour, IHealable, IAttackable, IPushable
{
    public Character character;

    public event Action CharacterDeathEvent;

    // Stats are accessed by other scripts need to be public.
    [HideInInspector] public string characterName; // from char scriptable object
    [HideInInspector] public Stat movementRange;
    [HideInInspector] public Stat strength;
    [HideInInspector] public Stat armor;
    [HideInInspector] public Stat intelligence;
    [HideInInspector] public Stat maxHealth;
    [HideInInspector] public Stat maxMana;

    public int currentHealth { get; private set; }
    public int currentMana { get; private set; }

    public List<Ability> abilities;

    DamageUI damageUI;

    CharacterRendererManager characterRendererManager;

    protected virtual void Awake()
    {
        damageUI = GetComponent<DamageUI>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }
    public void SetCharacteristics(Character _character)
    {
        character = _character;
        SetCharacteristics();
    }

    void SetCharacteristics()
    {
        // taking values from scriptable object to c#
        characterName = character.characterName;
        movementRange.baseValue = character.movementRange;
        strength.baseValue = character.strength; ;
        armor.baseValue = character.armor; ;
        intelligence.baseValue = character.intelligence; ;
        maxHealth.baseValue = character.maxHealth; ;
        maxMana.baseValue = character.maxMana;

        // TODO: /2 and startin mana is for heal testing purposes 
        currentHealth = maxHealth.GetValue();
        currentMana = 20;

        foreach (Ability ability in character.characterAbilities)
        {
            // I am cloning the ability coz if I don't there is only one scriptable object and it overrides variables
            // if 2 characters use the same ability
            var clone = Instantiate(ability);
            abilities.Add(clone);
            clone.Initialize(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();

        // to not repeat the code
        TakePiercingDamage(damage);
    }

    public void TakePiercingDamage(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;

        // displaying damage UI
        damageUI.DisplayDamage(damage);

        if (currentHealth <= 0)
            Die();
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana.GetValue());
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana.GetValue());

        if (amount == 0)
            return;
        Debug.Log(transform.name + " uses " + amount + " mana.");
    }

    public void GainHealth(int healthGain)
    {
        healthGain = Mathf.Clamp(healthGain, 0, maxHealth.GetValue() - currentHealth);
        currentHealth += healthGain;

        Debug.Log(transform.name + " heals " + healthGain + " .");

        damageUI.DisplayHeal(healthGain);
    }

    public void GetPushed()
    {
        // TODO: do this instead of pushable character.
        
    }

    public virtual void Die()
    {
        // playing death animation
        characterRendererManager.PlayDieAnimation();

        // kill all tweens
        DOTween.KillAll();

        // die in some way
        // this method is meant to be overwirtten
        Destroy(gameObject, 0.5f);

        // movement script needs to clear the highlight 
        if (CharacterDeathEvent != null)
            CharacterDeathEvent();
    }

}

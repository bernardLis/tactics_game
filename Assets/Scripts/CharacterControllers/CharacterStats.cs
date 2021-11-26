using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;

public class CharacterStats : MonoBehaviour, IHealable, IAttackable<GameObject>, IPushable<Vector3>
{
    public Character character;

    [Header("Base value for stats")]
    public int baseMaxHealth = 100;
    public int baseMaxMana = 50;
    public int baseArmor = 0;
    public int baseMovementRange = 5;

    // Stats are accessed by other scripts need to be public.
    [HideInInspector] public string characterName;
    [HideInInspector] public Stat strength;
    [HideInInspector] public Stat intelligence;
    [HideInInspector] public Stat agility;
    [HideInInspector] public Stat stamina;

    [HideInInspector] public Stat maxHealth;
    [HideInInspector] public Stat maxMana;

    [HideInInspector] public Stat armor;
    [HideInInspector] public Stat movementRange;

    // lists of abilities
    [Header("Filled with character SO")]
    public List<Stat> stats;
    public List<Ability> basicAbilities;
    public List<Ability> abilities;

    // local
    DamageUI damageUI;
    CharacterRendererManager characterRendererManager;
    AILerp aILerp;

    // pushable variables
    Vector3 startingPos;
    Vector3 finalPos;
    int characterDmg = 10;
    GameObject tempObject;

    public int currentHealth { get; private set; }
    public int currentMana { get; private set; }

    // reply to interaction
    bool isAttacker;

    // delegate
    public event Action CharacterDeathEvent;

    protected virtual void Awake()
    {
        damageUI = GetComponent<DamageUI>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        aILerp = GetComponent<AILerp>();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        AddStatsToList();
    }

    protected virtual void TurnManager_OnBattleStateChanged(BattleState state)
    {
        // TODO: modifiers should last number of turns and I should be checking each stat for modifier and how many turns are left;
        foreach (Stat stat in stats)
        {
            if (stat.modifiers.Count == 0)
                continue;

            // iterate from the back to remove safely.
            for (int i = stat.modifiers.Count; i <= 0; i--)
                stat.RemoveModifier(stat.modifiers[i]);
        }

        if (TurnManager.currentTurn == 1)
            return;

        GainMana(10);
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void AddStatsToList()
    {
        stats.Add(strength);
        stats.Add(intelligence);
        stats.Add(agility);
        stats.Add(stamina);

        stats.Add(maxHealth);
        stats.Add(maxMana);

        stats.Add(armor);
        stats.Add(movementRange);
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

        strength.baseValue = character.strength;
        intelligence.baseValue = character.intelligence;
        agility.baseValue = character.agility;
        stamina.baseValue = character.stamina;

        maxHealth.baseValue = baseMaxHealth + character.stamina * 5;
        maxMana.baseValue = baseMaxMana + character.intelligence * 5;

        armor.baseValue = baseArmor; // TODO: should be base value + all pieces of eq
        movementRange.baseValue = 5 + Mathf.FloorToInt(character.agility / 3);

        // TODO: /2 and startin mana is for heal testing purposes 
        currentHealth = maxHealth.GetValue();
        currentMana = 20;

        // set weapon for animations & deactivate the game object
        WeaponHolder wh = GetComponentInChildren<WeaponHolder>();
        wh.SetWeapon(character.weapon);
        wh.gameObject.SetActive(false);

        // adding basic attack from weapon to basic abilities to be instantiated
        if (character.weapon != null)
        {
            var clone = Instantiate(character.weapon.basicAttack);
            basicAbilities.Add(clone);
            clone.Initialize(gameObject);
        }

        foreach (Ability ability in character.basicAbilities)
        {
            // I am cloning the ability coz if I don't there is only one scriptable object and it overrides variables
            // if 2 characters use the same ability
            var clone = Instantiate(ability);
            basicAbilities.Add(clone);
            clone.Initialize(gameObject);
        }

        foreach (Ability ability in character.characterAbilities)
        {
            if (ability == null)
                continue;

            var clone = Instantiate(ability);
            abilities.Add(clone);
            clone.Initialize(gameObject);
        }
    }

    public async Task TakeDamage(int damage, GameObject attacker)
    {
        damage -= armor.GetValue();

        // to not repeat the code
        await TakePiercingDamage(damage, attacker);
    }

    public async Task TakePiercingDamage(int damage, GameObject attacker)
    {
        // TODO: chance to dodge
        // TODO: there should be undodgable damage - from push and traps 
        // TODO: there problably should be unreplieable damage


        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;

        // displaying damage UI
        damageUI.DisplayDamage(damage);

        // shake a character;
        float duration = 0.5f;
        float strength = 0.1f;
        transform.DOShakePosition(duration, strength);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (attacker == null)
            return;

        // blocking interaction replies to go on forever;
        if (isAttacker)
            return;

        // strike back triggering basic attack at him
        foreach (Ability a in basicAbilities)
        {
            // TODO: if attacker in range of basic attack
            if (a.aType == AbilityType.ATTACK)
                if (!await a.TriggerAbility(attacker))
                    return;
        }

        // it will go on until one of them is dead, need a flag to determine whether it is a retaliation or not
        //
        // problems:
        // 2. it will trigger for bow attacks through the whole map
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

        Debug.Log(transform.name + " heals " + healthGain + ".");

        damageUI.DisplayHeal(healthGain);
    }

    public void GetPushed(Vector3 _dir)
    {
        startingPos = transform.position;
        finalPos = transform.position + _dir;

        // TODO: do this instead of pushable character.
        StartCoroutine(MoveToPosition(finalPos, 0.5f));
        Invoke("CollisionCheck", 0.35f);
    }

    IEnumerator MoveToPosition(Vector3 finalPos, float time)
    {
        // TODO: this AILerp hack is meh
        tempObject = new("Dest");
        tempObject.transform.position = finalPos;
        GetComponent<AIDestinationSetter>().target = tempObject.transform;
        aILerp.canMove = false;

        GetComponent<AILerp>().enabled = false;
        Vector3 startingPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Update astar
        AstarPath.active.Scan();

        aILerp.enabled = true;
        aILerp.canMove = true;
        aILerp.Teleport(finalPos, true);

        if (tempObject != null)
            Destroy(tempObject);
    }

    void CollisionCheck()
    {
        // check what is in boulders new place and act accordingly
        BoxCollider2D characterCollider = transform.GetComponentInChildren<BoxCollider2D>();
        characterCollider.enabled = false;

        Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);

        if (col == null)
        {
            characterCollider.enabled = true;
            return;
        }

        // player/enemy get dmged by 10 and are moved back to their starting position
        // character colliders are children
        if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
        {
            TakePiercingDamage(characterDmg, null);

            CharacterStats targetStats = col.transform.parent.GetComponent<CharacterStats>();
            targetStats.TakePiercingDamage(characterDmg, null);

            // move back to starting position (if target is not dead)
            // TODO: test what happens when target dies
            if (targetStats.currentHealth <= 0)
            {
                if (characterCollider != null)
                    characterCollider.enabled = true;
                return;
            }

            if (tempObject != null)
                Destroy(tempObject);

            StartCoroutine(MoveToPosition(startingPos, 0.5f));
        }
        // character destroys boulder when they are pushed into it + 10dmg to self
        else if (col.transform.gameObject.CompareTag("Stone"))
        {
            TakePiercingDamage(characterDmg, null);

            Destroy(col.transform.parent.gameObject);
        }
        // character triggers traps
        else if (col.transform.gameObject.CompareTag("Trap"))
        {
            int dmg = col.transform.GetComponentInParent<FootholdTrap>().damage;

            TakePiercingDamage(dmg, null);
            // movement range is down by 1 for each trap enemy walks on
            movementRange.AddModifier(-1);

            Destroy(col.transform.parent.gameObject);
        }
        else
        {
            TakePiercingDamage(characterDmg, null);
            if (tempObject != null)
                Destroy(tempObject);
            StartCoroutine(MoveToPosition(startingPos, 0.5f));
        }
        // TODO: pushing characters into the river/other obstacles?
        // currently you can't target it on the river bank
        if (characterCollider != null)
            characterCollider.enabled = true;
    }

    public virtual void Die()
    {
        // playing death animation
        characterRendererManager.PlayDieAnimation();

        // ending that character's turn // TODO: is that a smart way to handle death?
        TurnManager.instance.PlayerCharacterTurnFinished();

        // kill all tweens TODO: is that OK?
        DOTween.KillAll();

        // die in some way
        // this method is meant to be overwirtten
        Destroy(gameObject, 0.5f);

        // movement script needs to clear the highlight 
        if (CharacterDeathEvent != null)
            CharacterDeathEvent();
    }

    public void SetAttacker(bool _isAttacker) { isAttacker = _isAttacker; }

}

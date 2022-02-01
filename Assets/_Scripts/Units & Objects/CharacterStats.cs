using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour, IHealable<Ability>, IAttackable<GameObject, Ability>, IPushable<Vector3, Ability>, IBuffable<Ability>
{
    public Character character;

    [Header("Base value for stats")]
    public int baseMaxHealth = 100;
    public int baseMaxMana = 50;
    public int baseArmor = 0;
    public int baseMovementRange = 5;

    // Stats are accessed by other scripts need to be public.
    [HideInInspector] public string characterName;

    // TODO: I could make them private and use only list to get info about stats
    [HideInInspector] public Stat strength = new(StatType.Strength);
    [HideInInspector] public Stat intelligence = new(StatType.Intelligence);
    [HideInInspector] public Stat agility = new(StatType.Agility);
    [HideInInspector] public Stat stamina = new(StatType.Stamina);

    [HideInInspector] public Stat maxHealth = new(StatType.MaxHealth);
    [HideInInspector] public Stat maxMana = new(StatType.MaxMana);

    [HideInInspector] public Stat armor = new(StatType.Armor);
    [HideInInspector] public Stat movementRange = new(StatType.MovementRange);

    // lists of abilities
    [Header("Filled on character init")]
    public List<Stat> stats;
    public List<Ability> basicAbilities;
    public List<Ability> abilities;

    // local
    DamageUI damageUI;
    CharacterRendererManager characterRendererManager;
    AILerp aILerp;

    // global
    Highlighter highlighter;

    // pushable variables
    Vector3 startingPos;
    Vector3 finalPos;
    int characterDmg = 10;
    GameObject tempObject;

    public int currentHealth { get; private set; }
    public int currentMana { get; private set; }

    // retaliation on interaction
    public bool isAttacker { get; private set; }

    // statuses
    public List<Status> statuses = new();
    public bool isStunned; //{ get; private set; }

    // delegate
    public event Action CharacterDeathEvent;
    protected virtual void Awake()
    {
        // local
        damageUI = GetComponent<DamageUI>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        aILerp = GetComponent<AILerp>();

        // global
        highlighter = Highlighter.instance;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        AddStatsToList();
    }

    protected virtual void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        foreach (Status s in statuses)
            if (s.ShouldTrigger())
                s.TriggerStatus();

        if (TurnManager.currentTurn <= 1)
            return;

        GainMana(10);
    }

    protected void ResolveModifiersTurnEnd()
    {
        // modifiers
        foreach (Stat s in stats)
            s.TurnEndDecrement();

        // statuses
        for (int i = statuses.Count - 1; i >= 0; i--)
        {
            statuses[i].ResolveTurnEnd();

            if (statuses[i].ShouldBeRemoved())
            {
                statuses[i].ResetFlag();
                statuses.Remove(statuses[i]);
            }
        }
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
        int mRangeCalculation = 5 + Mathf.FloorToInt(character.agility / 3);
        movementRange.baseValue = Mathf.Clamp(mRangeCalculation, 0, 9); // after 9 it lags unity.

        // TODO: startin mana is for heal testing purposes 
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

    public async Task TakeDamage(int _damage, GameObject _attacker, Ability _ability)
    {
        _damage -= armor.GetValue();

        // to not repeat the code
        await TakePiercingDamage(_damage, _attacker, _ability);
    }

    public async Task TakePiercingDamage(int _damage, GameObject _attacker, Ability _ability)
    {
        // in the side 1, face to face 2, from the back 0, 
        int attackDir = CalculateAttackDir(_attacker.transform.position);
        float dodgeChance = CalculateDodgeChance(attackDir, _attacker);
        float randomVal = Random.value;
        if (randomVal < dodgeChance && !isStunned) // dodgeChance% of time <- TODO: is that correct?
            Dodge(_attacker);
        else
        {
            TakeDamageNoDodgeNoRetaliation(_damage);
            HandleModifier(_ability);
            HandleStatus(_ability, _attacker);
        }

        if (currentHealth <= 0)
            return;

        if (!WillRetaliate(_attacker))
            return;

        if (_attacker == null)
            return;

        if (_attacker.GetComponent<CharacterStats>().currentHealth <= 0)
            return;

        // blocking interaction replies to go on forever;
        if (isAttacker)
            return;

        // it is just the basic attack that is not ranged;
        AttackAbility retaliationAbility = GetRetaliationAbility();
        if (retaliationAbility == null)
            return;

        // strike back triggering basic attack at him
        if (!retaliationAbility.CanHit(gameObject, _attacker))
            return;

        await Task.Delay(500);

        // if it is in range retaliate
        characterRendererManager.Face((_attacker.transform.position - transform.position).normalized);
        retaliationAbility.SetIsRetaliation(true);
        await retaliationAbility.TriggerAbility(_attacker);
    }

    void Dodge(GameObject _attacker)
    {
        // face the attacker
        characterRendererManager.Face((_attacker.transform.position - transform.position).normalized);

        damageUI.DisplayOnCharacter("Dodged!", 24, Color.black);

        // shake yourself
        float duration = 0.5f;
        float strength = 0.8f;

        // TODO: Dodged is bugged, it sometimes does not shake the character but just turns it around.
        characterRendererManager.enabled = false;
        transform.DOShakePosition(duration, strength, 0, 0, false, true)
                 .OnComplete(() => characterRendererManager.enabled = true);
    }

    public void TakeDamageNoDodgeNoRetaliation(int _damage)
    {
        _damage = Mathf.Clamp(_damage, 0, int.MaxValue);
        currentHealth -= _damage;

        // displaying damage UI
        damageUI.DisplayOnCharacter(_damage.ToString(), 36, Helpers.GetColor("damageRed"));

        // shake a character;
        float duration = 0.5f;
        float strength = 0.1f;

        // don't shake on death
        if (currentHealth <= 0)
        {
            Die().GetAwaiter();
            return;
        }

        // TODO: Dodged is bugged, it sometimes does not shake the character but just turns it around.
        characterRendererManager.enabled = false;
        transform.DOShakePosition(duration, strength)
                 .OnComplete(() => characterRendererManager.enabled = true);

    }

    public void GetBuffed(Ability _ability)
    {
        HandleModifier(_ability);
        HandleStatus(_ability, null);
    }

    void HandleModifier(Ability _ability)
    {
        if (_ability.statModifier != null)
        {
            AddModifier(_ability);
        }
    }

    void HandleStatus(Ability _ability, GameObject _attacker)
    {
        if (_ability.status != null)
            AddStatus(_ability.status, _attacker);
    }

    public int CalculateAttackDir(Vector3 _attackerPos)
    {
        // does not matter what dir is attacker facing, it matters where he stands
        // coz he will turn around when attacking to face the defender
        Vector2 attackerFaceDir = (transform.position - _attackerPos).normalized;
        Vector2 defenderFaceDir = characterRendererManager.faceDir;

        // in the side 1, face to face 2, from the back 0, 
        int attackDir = 1;
        if (attackerFaceDir + defenderFaceDir == Vector2.zero)
            attackDir = 2;
        if (attackerFaceDir + defenderFaceDir == attackerFaceDir * 2)
            attackDir = 0;

        return attackDir;
    }

    float CalculateDodgeChance(int _attackDir, GameObject _attacker)
    {
        // base 50% chance of dodging when being attacked face to face
        // base 25% chance of dodging when being attacked from the side
        // base 0% chance of dodging when being attacked from the back

        // additionally, every point difference in agi between characters is worth 2%
        // if it is negative, than attacker is more agile than us, so higher chance to hit.

        int agiDiff = agility.GetValue() - _attacker.GetComponent<CharacterStats>().agility.GetValue();

        return (float)(0.25 * _attackDir) + (float)(0.02 * agiDiff);
    }

    public AttackAbility GetRetaliationAbility()
    {
        foreach (Ability a in basicAbilities)
        {
            // no retaliation with ranged attacks 
            if (a.weaponType == WeaponType.SHOOT)
                continue;

            if (a.aType != AbilityType.Attack)
                continue;

            return (AttackAbility)a;
        }

        return null;
    }

    // used by info card ui
    public bool WillRetaliate(GameObject _attacker)
    {
        if (isStunned)
            return false;

        // if attacked from the back, don't retaliate
        if (CalculateAttackDir(_attacker.transform.position) == 0)
            return false;

        // if attacker is yourself, don't retaliate
        if (_attacker == gameObject)
            return false;

        return true;
    }

    public float GetDodgeChance(GameObject _attacker)
    {
        if (isStunned)
            return 0;

        return CalculateDodgeChance(CalculateAttackDir(_attacker.transform.position), _attacker);
    }

    public void GainMana(int _amount)
    {
        currentMana += _amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana.GetValue());
    }

    public void UseMana(int _amount)
    {
        currentMana -= _amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana.GetValue());

        if (_amount == 0)
            return;
        Debug.Log(transform.name + " uses " + _amount + " mana.");
    }

    public void GainHealth(int _healthGain, Ability _ability)
    {
        _healthGain = Mathf.Clamp(_healthGain, 0, maxHealth.GetValue() - currentHealth);
        currentHealth += _healthGain;

        Debug.Log(transform.name + " heals " + _healthGain + ".");

        HandleModifier(_ability);
        HandleStatus(_ability, null);

        damageUI.DisplayOnCharacter(_healthGain.ToString(), 36, Helpers.GetColor("healthGainGreen"));
    }

    public void GetPushed(Vector3 _dir, Ability _ability)
    {
        startingPos = transform.position;
        finalPos = transform.position + _dir;

        HandleModifier(_ability);
        HandleStatus(_ability, null);

        // TODO: do this instead of pushable character.
        StartCoroutine(MoveToPosition(finalPos, 0.5f));
        Invoke("CollisionCheck", 0.35f);
    }

    IEnumerator MoveToPosition(Vector3 _finalPos, float _time)
    {
        // TODO: this AILerp hack is meh
        tempObject = new("Dest");
        tempObject.transform.position = _finalPos;
        GetComponent<AIDestinationSetter>().target = tempObject.transform;

        aILerp.canMove = false;
        aILerp.enabled = false;
        Vector3 startingPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < _time)
        {
            transform.position = Vector3.Lerp(startingPos, _finalPos, (elapsedTime / _time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Update astar
        AstarPath.active.Scan();

        aILerp.enabled = true;
        aILerp.canMove = true;
        aILerp.Teleport(_finalPos, true);

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
            TakeDamageNoDodgeNoRetaliation(characterDmg);

            CharacterStats targetStats = col.transform.parent.GetComponent<CharacterStats>();
            targetStats.TakeDamageNoDodgeNoRetaliation(characterDmg);

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
        else if (col.transform.gameObject.CompareTag("PushableObstacle"))
        {
            TakeDamageNoDodgeNoRetaliation(characterDmg);

            Destroy(col.transform.parent.gameObject);
        }
        // character triggers traps
        else if (col.transform.gameObject.CompareTag("Trap"))
        {
            int dmg = col.transform.GetComponentInParent<FootholdTrap>().damage;

            TakeDamageNoDodgeNoRetaliation(dmg);
            // movement range is down by 1 for each trap enemy walks on
            //movementRange.AddModifier(-1);

            Destroy(col.transform.parent.gameObject);
        }
        else
        {
            TakeDamageNoDodgeNoRetaliation(characterDmg);
            if (tempObject != null)
                Destroy(tempObject);
            StartCoroutine(MoveToPosition(startingPos, 0.5f));
        }
        // TODO: pushing characters into the river/other obstacles?
        // currently you can't target it on the river bank
        if (characterCollider != null)
            characterCollider.enabled = true;
    }

    public async Task Die()
    {
        await highlighter.ClearHighlightedTiles();

        // playing death animation
        await characterRendererManager.Die();

        // ending that character's turn // TODO: is that a smart way to handle death?
        TurnManager.instance.PlayerCharacterTurnFinished();

        // kill all tweens TODO: is that OK?
        DOTween.KillAll();

        // movement script needs to clear the highlight 
        if (CharacterDeathEvent != null)
            CharacterDeathEvent();

        // die in some way
        // this method is meant to be overwirtten
        Destroy(gameObject);
    }

    public void SetAttacker(bool _is) { isAttacker = _is; }
    public void SetIsStunned(bool _is) { isStunned = _is; }

    void AddModifier(Ability _ability)
    {
        foreach (Stat s in stats)
            if (s.type == _ability.statModifier.statType)
                s.AddModifier(Instantiate(_ability.statModifier)); // stat checks if modifier is a dupe, to prevent stacking

    }

    public void AddStatus(Status _s, GameObject _attacker)
    {
        // statuses don't stack
        if (IsStatusOn(_s))
            return;

        var clone = Instantiate(_s);
        statuses.Add(clone);
        clone.Initialize(gameObject, _attacker);
        // status triggers right away
        clone.FirstTrigger();
    }

    bool IsStatusOn(Status _s)
    {
        foreach (Status s in statuses)
            if (s.id == _s.id)
                return true;

        return false;
    }
}

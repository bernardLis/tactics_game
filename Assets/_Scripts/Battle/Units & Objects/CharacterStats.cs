using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour, IHealable<GameObject, Ability>, IAttackable<GameObject, Ability>, IPushable<GameObject, Vector3, Ability>, IBuffable<GameObject, Ability>
{
    [HideInInspector] public Character Character { get; private set; }

    // Stats are accessed by other scripts need to be public.
    // TODO: I could make them private and use only list to get info about stats
    [HideInInspector] public Stat Strength = new();
    [HideInInspector] public Stat Intelligence = new();
    [HideInInspector] public Stat Agility = new();
    [HideInInspector] public Stat Stamina = new();
    [HideInInspector] public Stat MaxHealth = new();
    [HideInInspector] public Stat MaxMana = new();
    [HideInInspector] public Stat Armor = new();
    [HideInInspector] public Stat MovementRange = new();

    // lists of abilities
    [Header("Filled on character init")]
    [HideInInspector] public List<Stat> Stats = new();
    [HideInInspector] public List<Ability> BasicAbilities = new();
    [HideInInspector] public List<Ability> Abilities = new();

    // local
    DamageUI _damageUI;
    CharacterRendererManager _characterRendererManager;
    AILerp _AILerp;

    // global
    Highlighter _highlighter;

    // pushable variables
    Vector3 _startingPos;
    Vector3 _finalPos;
    int _characterDmg = 10;
    GameObject _tempObject;

    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }

    // retaliation on interaction
    public bool IsAttacker { get; private set; }

    // statuses
    [HideInInspector] public List<Status> Statuses = new();
    public bool IsStunned { get; private set; }

    // delegate
    public event Action<GameObject> CharacterDeathEvent;
    protected virtual void Awake()
    {
        // local
        _damageUI = GetComponent<DamageUI>();
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        _AILerp = GetComponent<AILerp>();

        // global
        _highlighter = Highlighter.instance;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        AddStatsToList();
    }

    protected virtual void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        foreach (Status s in Statuses)
            if (s.ShouldTrigger())
                s.TriggerStatus();

        if (TurnManager.CurrentTurn <= 1)
            return;

        GainMana(10);
    }

    protected void ResolveModifiersTurnEnd()
    {
        // modifiers
        foreach (Stat s in Stats)
            s.TurnEndDecrement();

        // statuses
        for (int i = Statuses.Count - 1; i >= 0; i--)
        {
            Statuses[i].ResolveTurnEnd();

            if (Statuses[i].ShouldBeRemoved())
            {
                Statuses[i].ResetFlag();
                Statuses.Remove(Statuses[i]);
            }
        }
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    // Enemy creation from board manager uses that
    public void SetCharacteristics(Character character)
    {
        Character = character;
        SetCharacteristics();
    }

    void SetCharacteristics()
    {
        // taking values from scriptable object to c#
        Strength.Initialize(StatType.Strength, Character.Strength);
        Intelligence.Initialize(StatType.Intelligence, Character.Intelligence);
        Agility.Initialize(StatType.Agility, Character.Agility);
        Stamina.Initialize(StatType.Stamina, Character.Stamina);

        Character.UpdateDerivativeStats();
        MaxHealth.Initialize(StatType.MaxHealth, Character.MaxHealth);
        MaxMana.Initialize(StatType.MaxMana, Character.MaxMana);
        Armor.Initialize(StatType.Armor, Character.Armor);
        MovementRange.Initialize(StatType.MovementRange, Character.MovementRange);

        // TODO: starting mana is for testing purposes 
        CurrentHealth = MaxHealth.GetValue();
        CurrentMana = 20;

        // set weapon for animations & deactivate the game object
        WeaponHolder wh = GetComponentInChildren<WeaponHolder>();
        wh.SetWeapon(Character.Weapon);
        wh.gameObject.SetActive(false);

        // adding basic attack from weapon to basic abilities to be instantiated
        if (Character.Weapon != null)
        {
            var clone = Instantiate(Character.Weapon.BasicAttack);
            BasicAbilities.Add(clone);
            clone.Initialize(gameObject);
        }

        foreach (Ability ability in Character.BasicAbilities)
        {
            // I am cloning the ability coz if I don't there is only one scriptable object and it overrides variables
            // if 2 characters use the same ability
            var clone = Instantiate(ability);
            BasicAbilities.Add(clone);
            clone.Initialize(gameObject);
        }

        foreach (Ability ability in Character.Abilities)
        {
            if (ability == null)
                continue;

            var clone = Instantiate(ability);
            Abilities.Add(clone);
            clone.Initialize(gameObject);
        }
    }

    void AddStatsToList()
    {
        Stats.Clear();

        Stats.Add(Strength);
        Stats.Add(Intelligence);
        Stats.Add(Agility);
        Stats.Add(Stamina);

        Stats.Add(MaxHealth);
        Stats.Add(MaxMana);
        Stats.Add(Armor);
        Stats.Add(MovementRange);
    }

    public async Task TakeDamage(int damage, GameObject attacker, Ability ability)
    {
        // to not repeat the code
        await TakePiercingDamage(damage, attacker, ability);
    }

    public async Task TakePiercingDamage(int damage, GameObject attacker, Ability ability)
    {
        // in the side 1, face to face 2, from the back 0, 
        int attackDir = CalculateAttackDir(attacker.transform.position);
        float dodgeChance = CalculateDodgeChance(attackDir, attacker);
        float randomVal = Random.value;
        if (randomVal < dodgeChance && !IsStunned) // dodgeChance% of time <- TODO: is that correct?
            Dodge(attacker);
        else
        {
            TakeDamageNoDodgeNoRetaliation(damage);
            HandleModifier(ability);
            HandleStatus(attacker, ability);
        }

        if (CurrentHealth <= 0)
            return;

        if (!WillRetaliate(attacker))
            return;

        if (attacker == null)
            return;

        if (attacker.GetComponent<CharacterStats>().CurrentHealth <= 0)
            return;

        // blocking interaction replies to go on forever;
        if (IsAttacker)
            return;

        // it is just the basic attack that is not ranged;
        AttackAbility retaliationAbility = GetRetaliationAbility();
        if (retaliationAbility == null)
            return;

        // strike back triggering basic attack at him
        if (!retaliationAbility.CanHit(gameObject, attacker))
            return;

        await Task.Delay(500);

        // if it is in range retaliate
        _characterRendererManager.Face((attacker.transform.position - transform.position).normalized);
        retaliationAbility.SetIsRetaliation(true);
        await retaliationAbility.TriggerAbility(attacker);
    }

    void Dodge(GameObject attacker)
    {
        // face the attacker
        _characterRendererManager.Face((attacker.transform.position - transform.position).normalized);

        _damageUI.DisplayOnCharacter("Dodged!", 24, Color.black);

        // shake yourself
        float duration = 0.5f;
        float strength = 0.8f;

        // TODO: Dodged is bugged, it sometimes does not shake the character but just turns it around.
        _characterRendererManager.enabled = false;
        transform.DOShakePosition(duration, strength, 0, 0, false, true)
                 .OnComplete(() => _characterRendererManager.enabled = true);
    }

    public void TakeDamageNoDodgeNoRetaliation(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        CurrentHealth -= damage;

        // displaying damage UI
        _damageUI.DisplayOnCharacter(damage.ToString(), 36, Helpers.GetColor("damageRed"));

        // shake a character;
        float duration = 0.5f;
        float strength = 0.1f;

        // don't shake on death
        if (CurrentHealth <= 0)
        {
            Die().GetAwaiter();
            return;
        }

        _characterRendererManager.enabled = false;
        transform.DOShakePosition(duration, strength)
                 .OnComplete(() => _characterRendererManager.enabled = true);

    }

    public void GetBuffed(GameObject attacker, Ability ability)
    {
        HandleModifier(ability);
        HandleStatus(attacker, ability);
    }

    void HandleModifier(Ability ability)
    {
        if (ability.StatModifier != null)
            AddModifier(ability);
    }

    void HandleStatus(GameObject attacker, Ability ability)
    {
        if (ability.Status != null)
            AddStatus(ability.Status, attacker);
    }

    public int CalculateAttackDir(Vector3 attackerPos)
    {
        // does not matter what dir is attacker facing, it matters where he stands
        // coz he will turn around when attacking to face the defender
        Vector2 attackerFaceDir = (transform.position - attackerPos).normalized;
        Vector2 defenderFaceDir = _characterRendererManager.FaceDir;

        // in the side 1, face to face 2, from the back 0, 
        int attackDir = 1;
        if (attackerFaceDir + defenderFaceDir == Vector2.zero)
            attackDir = 2;
        if (attackerFaceDir + defenderFaceDir == attackerFaceDir * 2)
            attackDir = 0;

        return attackDir;
    }

    float CalculateDodgeChance(int attackDir, GameObject attacker)
    {
        // base 50% chance of dodging when being attacked face to face
        // base 25% chance of dodging when being attacked from the side
        // base 0% chance of dodging when being attacked from the back

        // additionally, every point difference in agi between characters is worth 2%
        // if it is negative, than attacker is more agile than us, so higher chance to hit.

        int agiDiff = Agility.GetValue() - attacker.GetComponent<CharacterStats>().Agility.GetValue();

        return (float)(0.25 * attackDir) + (float)(0.02 * agiDiff);
    }

    public AttackAbility GetRetaliationAbility()
    {
        foreach (Ability a in BasicAbilities)
        {
            // no retaliation with ranged attacks 
            if (a.WeaponType == WeaponType.Shoot)
                continue;

            if (a.AbilityType != AbilityType.Attack)
                continue;

            return (AttackAbility)a;
        }

        return null;
    }

    // used by info card ui
    public bool WillRetaliate(GameObject attacker)
    {
        if (IsStunned)
            return false;

        // if attacked from the back, don't retaliate
        if (CalculateAttackDir(attacker.transform.position) == 0)
            return false;

        // if attacker is yourself, don't retaliate
        if (attacker == gameObject)
            return false;

        return true;
    }

    public float GetDodgeChance(GameObject attacker)
    {
        if (IsStunned)
            return 0;

        return CalculateDodgeChance(CalculateAttackDir(attacker.transform.position), attacker);
    }

    public void GainMana(int amount)
    {
        CurrentMana += amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());
    }

    public void UseMana(int amount)
    {
        CurrentMana -= amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());

        if (amount == 0)
            return;
    }

    public void GainHealth(int healthGain, GameObject attacker, Ability ability)
    {
        healthGain = Mathf.Clamp(healthGain, 0, MaxHealth.GetValue() - CurrentHealth);
        CurrentHealth += healthGain;

        HandleModifier(ability);
        HandleStatus(attacker, ability);

        _damageUI.DisplayOnCharacter(healthGain.ToString(), 36, Helpers.GetColor("healthGainGreen"));
    }

    public void GetPushed(GameObject attacker, Vector3 dir, Ability ability)
    {
        _startingPos = transform.position;
        _finalPos = transform.position + dir;

        HandleModifier(ability);
        HandleStatus(attacker, ability);

        // TODO: do this instead of pushable character.
        StartCoroutine(MoveToPosition(_finalPos, 0.5f));
        Invoke("CollisionCheck", 0.35f);
    }

    IEnumerator MoveToPosition(Vector3 finalPos, float time)
    {
        // TODO: this AILerp hack is meh
        _tempObject = new("Dest");
        _tempObject.transform.position = finalPos;
        GetComponent<AIDestinationSetter>().target = _tempObject.transform;

        _AILerp.canMove = false;
        _AILerp.enabled = false;
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

        _AILerp.enabled = true;
        _AILerp.canMove = true;
        _AILerp.Teleport(finalPos, true);

        if (_tempObject != null)
            Destroy(_tempObject);
    }

    void CollisionCheck()
    {
        // check what is in boulders new place and act accordingly
        BoxCollider2D characterCollider = transform.GetComponentInChildren<BoxCollider2D>();
        characterCollider.enabled = false;

        Collider2D col = Physics2D.OverlapCircle(_finalPos, 0.2f);

        if (col == null)
        {
            characterCollider.enabled = true;
            return;
        }

        // player/enemy get dmged by 10 and are moved back to their starting position
        // character colliders are children
        if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
        {
            TakeDamageNoDodgeNoRetaliation(_characterDmg);

            CharacterStats targetStats = col.transform.parent.GetComponent<CharacterStats>();
            targetStats.TakeDamageNoDodgeNoRetaliation(_characterDmg);

            // move back to starting position (if target is not dead)
            // TODO: test what happens when target dies
            if (targetStats.CurrentHealth <= 0)
            {
                if (characterCollider != null)
                    characterCollider.enabled = true;
                return;
            }

            if (_tempObject != null)
                Destroy(_tempObject);

            StartCoroutine(MoveToPosition(_startingPos, 0.5f));
        }
        // character destroys boulder when they are pushed into it + 10dmg to self
        else if (col.transform.gameObject.CompareTag("PushableObstacle"))
        {
            TakeDamageNoDodgeNoRetaliation(_characterDmg);

            Destroy(col.transform.parent.gameObject);
        }
        // character triggers traps
        else if (col.transform.gameObject.CompareTag("Trap"))
        {
            int dmg = col.transform.GetComponentInParent<FootholdTrap>().Damage;

            TakeDamageNoDodgeNoRetaliation(dmg);
            // movement range is down by 1 for each trap enemy walks on
            //movementRange.AddModifier(-1);

            Destroy(col.transform.parent.gameObject);
        }
        else
        {
            TakeDamageNoDodgeNoRetaliation(_characterDmg);
            if (_tempObject != null)
                Destroy(_tempObject);
            StartCoroutine(MoveToPosition(_startingPos, 0.5f));
        }
        // TODO: pushing characters into the river/other obstacles?
        // currently you can't target it on the river bank
        if (characterCollider != null)
            characterCollider.enabled = true;
    }

    public async Task Die()
    {
        await _highlighter.ClearHighlightedTiles();

        // playing death animation
        await _characterRendererManager.Die();

        // kill all tweens TODO: is that OK?
        DOTween.KillAll();

        // movement script needs to clear the highlight 
        if (CharacterDeathEvent != null)
            CharacterDeathEvent(gameObject);

        // die in some way
        // this method is meant to be overwirtten
        Destroy(gameObject);
    }

    public void SetAttacker(bool isAttacker) { IsAttacker = isAttacker; }
    public void SetIsStunned(bool isStunned) { IsStunned = isStunned; }

    void AddModifier(Ability ability)
    {
        foreach (Stat s in Stats)
            if (s.Type == ability.StatModifier.StatType)
                s.AddModifier(Instantiate(ability.StatModifier)); // stat checks if modifier is a dupe, to prevent stacking
    }

    public void AddStatus(Status s, GameObject attacker)
    {
        // statuses don't stack
        if (IsStatusOn(s))
            return;

        var clone = Instantiate(s);
        Statuses.Add(clone);
        clone.Initialize(gameObject, attacker);
        // status triggers right away
        clone.FirstTrigger();
    }

    bool IsStatusOn(Status s)
    {
        foreach (Status status in Statuses)
            if (status.Id == s.Id)
                return true;

        return false;
    }
}

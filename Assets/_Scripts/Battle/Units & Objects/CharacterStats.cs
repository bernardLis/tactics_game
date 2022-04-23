using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour, IHealable<GameObject, Ability>, IAttackable<GameObject, Ability>, IPushable<Vector3, GameObject, Ability>, IBuffable<GameObject, Ability>
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
    AILerp _aiLerp;

    // global
    Highlighter _highlighter;

    // pushable variables
    Vector3 _startingPos;
    Vector3 _finalPos;
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
        _aiLerp = GetComponent<AILerp>();

        // global
        _highlighter = Highlighter.Instance;

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
        // in the side 1, face to face 2, from the back 0, 
        int attackDir = CalculateAttackDir(attacker.transform.position);
        float dodgeChance = CalculateDodgeChance(attackDir, attacker);
        float randomVal = Random.value;
        bool dodged = false;

        if (randomVal < dodgeChance && !IsStunned)
        {
            // dodgeChance% of time <- TODO: is that correct?
            Dodge(attacker);
            dodged = true;
        }
        else
        {
            await TakeDamageNoDodgeNoRetaliation(damage);
            HandleModifier(ability);
            HandleStatus(attacker, ability);
        }

        if (attackDir == 0 && dodged)
            return;

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

        // TODO: kind of a hack to make retaliation work with my set-up
        Vector3 tilePos = BattleManager.Instance.GetComponent<TileManager>().Tilemap.WorldToCell(attacker.transform.position);
        WorldTile tile;
        if (!TileManager.Tiles.TryGetValue(tilePos, out tile))
            return;
        List<WorldTile> tiles = new();
        tiles.Add(tile);

        await retaliationAbility.TriggerAbility(tiles);
    }

    void Dodge(GameObject attacker)
    {
        // face the attacker
        _characterRendererManager.Face((attacker.transform.position - transform.position).normalized);

        _damageUI.DisplayOnCharacter("Dodged!", 24, Color.black);

        // shake yourself
        float duration = 0.5f;
        float strength = 0.8f;

        DisableAILerp();
        transform.DOShakePosition(duration, strength, 0, 0, false, true)
                 .OnComplete(() => EnableAILerp());
    }

    public async Task TakeDamageNoDodgeNoRetaliation(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        CurrentHealth -= damage;

        // displaying damage UI
        _damageUI.DisplayOnCharacter(damage.ToString(), 36, Helpers.GetColor("damageRed"));

        // don't shake on death
        if (CurrentHealth <= 0)
        {
            await Die();
            return;
        }

        // shake a character;
        float duration = 0.5f;
        float strength = 0.1f;

        DisableAILerp();
        transform.DOShakePosition(duration, strength)
                 .OnComplete(() => EnableAILerp());
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
        Vector2 attackerFaceDir = (attackerPos - transform.position).normalized; // i swapped attacker pos with transform.pos
        Vector2 defenderFaceDir = _characterRendererManager.GetFaceDir();

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

    public async Task GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _startingPos = transform.position;
        _finalPos = transform.position + dir;

        HandleModifier(ability);
        HandleStatus(attacker, ability);

        BoxCollider2D selfCollider = transform.GetComponentInChildren<BoxCollider2D>();
        selfCollider.enabled = false;
        Collider2D col = Physics2D.OverlapCircle(_finalPos, 0.2f);

        if (col == null)
            selfCollider.enabled = true;

        await MoveToPosition(_finalPos, 0.5f);
        await CheckCollision(ability, col);

        if (selfCollider != null)
            selfCollider.enabled = true;
    }

    public async Task MoveToPosition(Vector3 finalPos, float time)
    {
        // TODO: this AILerp hack is meh

        _tempObject = new("Dest");
        _tempObject.transform.position = finalPos;
        _aiLerp.destination = _tempObject.transform.position;
        DisableAILerp();

        Vector3 startingPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        _aiLerp.Teleport(finalPos, true);

        if (_tempObject != null)
            Destroy(_tempObject);
    }


    public async Task CheckCollision(Ability ability, Collider2D col)
    {
        // nothing to collide with = being pushed into empty space
        if (col == null)
            return;

        // player/enemy get damaged  and are moved back to their starting position
        // character colliders are children
        if (col.CompareTag(Tags.Player) || col.transform.gameObject.CompareTag(Tags.Enemy))
            await CollideWithCharacter(ability, col);

        // character bounces back from being pushed into obstacle (and takes damage)
        if (col.CompareTag(Tags.Obstacle) || col.CompareTag(Tags.BoundCollider))
            await CollideWithIndestructible(ability, col);

        // character destroys boulder when they are pushed into it
        if (col.CompareTag(Tags.PushableObstacle))
            await CollideWithDestructible(ability, col);
    }

    public async Task CollideWithCharacter(Ability ability, Collider2D col)
    {
        await TakeDamageNoDodgeNoRetaliation(ability.BasePower);

        CharacterStats targetStats = col.GetComponent<CharacterStats>();
        await targetStats.TakeDamageNoDodgeNoRetaliation(ability.BasePower);

        if (_tempObject != null)
            Destroy(_tempObject);

        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithIndestructible(Ability ability, Collider2D col)
    {
        await TakeDamageNoDodgeNoRetaliation(ability.BasePower);
        if (_tempObject != null)
            Destroy(_tempObject);

        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithDestructible(Ability ability, Collider2D col)
    {
        Destroy(col.transform.parent.gameObject);

        await TakeDamageNoDodgeNoRetaliation(ability.BasePower);
    }

    public async Task Die()
    {
        // playing death animation
        await _characterRendererManager.Die();

        // kill all tweens TODO: is that OK?
        transform.DOKill();

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
        // statuses don't stack, they are refreshed
        RemoveOldStatus(s);

        var clone = Instantiate(s);
        Statuses.Add(clone);
        clone.Initialize(gameObject, attacker);
        // status triggers right away
        clone.FirstTrigger();
    }

    void RemoveOldStatus(Status s)
    {
        Status toRemove = null;
        foreach (Status status in Statuses)
            if (status.ReferenceID == s.ReferenceID)
                toRemove = status;

        if (toRemove != null)
            Statuses.Remove(toRemove);
    }

    protected void DisableAILerp()
    {
        _aiLerp.enabled = false;
        _aiLerp.canMove = false;
    }

    protected void EnableAILerp()
    {
        _aiLerp.enabled = true;
        _aiLerp.canMove = true;
    }
}

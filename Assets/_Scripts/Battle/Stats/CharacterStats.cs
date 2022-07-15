using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class CharacterStats : BaseStats, IHealable<GameObject, Ability>, IAttackable<GameObject, Ability>, IPushable<Vector3, GameObject, Ability>, IBuffable<GameObject, Ability>
{
    // global
    BattleCharacterController _battleCharacterController;
    AudioManager _audioManager;

    // local
    CharacterRendererManager _characterRendererManager;
    AILerp _aiLerp;

    [HideInInspector] public Character Character { get; private set; }

    // Stats are accessed by other scripts need to be public.
    // TODO: I could make them private and use only list to get info about stats
    [HideInInspector] public Stat Power = new();
    [HideInInspector] public Stat Agility = new();
    [HideInInspector] public Stat MaxHealth = new();
    [HideInInspector] public Stat MaxMana = new();
    [HideInInspector] public Stat Armor = new();
    [HideInInspector] public Stat MovementRange = new();

    // lists of abilities
    [Header("Filled on character init")]
    [HideInInspector] public List<Stat> Stats = new();
    [HideInInspector] public List<Ability> BasicAbilities = new();
    [HideInInspector] public List<Ability> Abilities = new();

    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }

    // retaliation on interaction
    public bool IsAttacker { get; private set; }

    // pushable variables
    Vector3 _startingPos;
    Vector3 _finalPos;
    GameObject _tempObject;

    // dmg
    [SerializeField] GameObject _deathEffect;
    [SerializeField] GameObject _body;
    Path _latPath;

    // statuses
    public int DamageReceivedWhenWalking { get; private set; }
    List<Status> _statusesBeforeWalking = new();
    List<Status> _statusesAddedWhenWalking = new();

    // place of power
    Ability _replacedAbility;

    // character progression
    CharacterStats _lastAttacker;
    [SerializeField] GameObject _levelUpEffect;

    // delegates
    public event Action<GameObject> OnCharacterDeath;
    public event Action<int, int, int> OnHealthChange; // total,  value before change, change 
    public event Action<int, int, int> OnManaChange;
    public event Action<StatModifier> OnModifierAdded;
    public event Action<Ability> OnAbilityAdded;


    protected override void Awake()
    {
        base.Awake();
        _battleCharacterController = BattleCharacterController.Instance;
        _audioManager = AudioManager.Instance;

        // local
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        _aiLerp = GetComponent<AILerp>();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        AddStatsToList();
    }

    protected virtual void TurnManager_OnBattleStateChanged(BattleState state)
    {
        foreach (Status s in Statuses)
            if (s.ShouldTrigger())
                s.TriggerStatus();

        if (TurnManager.CurrentTurn <= 1)
            return;

        GainMana(10);

        // resetting status flags
        _statusesAddedWhenWalking.Clear();
        DamageReceivedWhenWalking = 0;
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
        gameObject.name = character.CharacterName;
        Character = character;
        Character.OnCharacterLevelUp += OnCharacterLevelUp;
        Character.OnCharacterExpGain += OnCharacterExpGain;
        SetCharacteristics();
    }

    void OnCharacterLevelUp()
    {
        // spawn effect
        Destroy(Instantiate(_levelUpEffect, transform.position + Vector3.up, Quaternion.identity), 2f);
        CurrentHealth = Character.MaxHealth;
        _damageUI.DisplayOnCharacter("Level up!", 24, Color.black);
    }

    void OnCharacterExpGain(int gain)
    {
        _damageUI.DisplayOnCharacter($"+{gain} exp", 24, Color.white);
    }

    void SetCharacteristics()
    {
        // taking values from scriptable object to c#
        Power.Initialize(StatType.Power, Character.Power, Character);
        Agility.Initialize(StatType.Agility, Character.Agility, Character);

        Character.UpdateDerivativeStats();
        MaxHealth.Initialize(StatType.MaxHealth, Character.MaxHealth, Character);
        MaxMana.Initialize(StatType.MaxMana, Character.MaxMana, Character);
        Armor.Initialize(StatType.Armor, Character.Armor, Character);
        MovementRange.Initialize(StatType.MovementRange, Character.MovementRange, Character);

        CurrentHealth = MaxHealth.GetValue();
        CurrentMana = 0;

        // adding basic attack from weapon to basic abilities to be instantiated
        if (Character.Weapon != null)
        {
            // set weapon for animations & deactivate the game object
            WeaponHolder wh = GetComponentInChildren<WeaponHolder>();
            wh.SetWeapon(Character.Weapon);
            wh.gameObject.SetActive(false);

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

    public void SetCurrentHealth(int health)
    {
        CurrentHealth = health;
    }

    void AddStatsToList()
    {
        Stats.Clear();

        Stats.Add(Power);
        Stats.Add(Agility);

        Stats.Add(MaxHealth);
        Stats.Add(MaxMana);
        Stats.Add(Armor);
        Stats.Add(MovementRange);
    }

    public void Select()
    {
        _statusesBeforeWalking = new(Statuses);
    }

    public async Task<bool> TakeDamage(int damage, GameObject attacker, Ability ability)
    {
        bool wasAttackSuccesful = false;

        // in the side 1, face to face 2, from the back 0, 
        int attackDir = CalculateAttackDir(attacker);
        float dodgeChance = CalculateDodgeChance(attackDir, attacker, false);
        float randomVal = Random.value;

        // dodgeChance% of time <- TODO: is that correct?
        if (randomVal < dodgeChance && !IsStunned)
            Dodge(attacker);
        else
        {
            wasAttackSuccesful = true;
            _lastAttacker = attacker.GetComponent<CharacterStats>();
            await TakeDamageFinal(damage);
            HandleModifier(ability);
            HandleStatus(attacker, ability);
        }

        if (attackDir == 0)
            return wasAttackSuccesful;

        if (CurrentHealth <= 0)
            return wasAttackSuccesful;

        if (!WillRetaliate(attacker))
            return wasAttackSuccesful;

        if (attacker == null)
            return wasAttackSuccesful;

        if (attacker.GetComponent<CharacterStats>().CurrentHealth <= 0)
            return wasAttackSuccesful;

        // blocking interaction replies to go on forever;
        if (IsAttacker)
            return wasAttackSuccesful;

        // it is just the basic attack that is not ranged;
        AttackAbility retaliationAbility = GetRetaliationAbility();
        if (retaliationAbility == null)
            return wasAttackSuccesful;

        // strike back triggering basic attack at him
        if (!retaliationAbility.CanHit(gameObject, attacker))
            return wasAttackSuccesful;

        await Task.Delay(500);

        // if it is in range retaliate
        _characterRendererManager.Face((attacker.transform.position - transform.position).normalized);
        retaliationAbility.SetIsRetaliation(true);

        // TODO: kind of a hack to make retaliation work with my set-up
        Vector3 tilePos = BattleManager.Instance.GetComponent<TileManager>().Tilemap.WorldToCell(attacker.transform.position);
        WorldTile tile;
        if (!TileManager.Tiles.TryGetValue(tilePos, out tile))
            return wasAttackSuccesful;
        List<WorldTile> tiles = new();
        tiles.Add(tile);

        await retaliationAbility.TriggerAbility(tiles);
        return wasAttackSuccesful;
    }

    async void Dodge(GameObject attacker)
    {
        // face the attacker
        _characterRendererManager.Face((attacker.transform.position - transform.position).normalized);

        _damageUI.DisplayOnCharacter("Dodged!", 24, Color.black);
        _audioManager.PlaySFX("Dodge", transform.position);

        // shake yourself
        float duration = 0.5f;
        float strength = 0.8f;

        _body.transform.DOShakePosition(duration, strength, 0, 0, false, true);
        await Task.Delay(500);
    }

    void ShieldDamage()
    {
        _damageUI.DisplayOnCharacter("Shielded!", 24, Color.magenta);
    }

    public async Task TakeDamageFinal(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // displaying damage UI
        _damageUI.DisplayOnCharacter(damage.ToString(), 36, Helpers.GetColor("damageRed"));

        OnHealthChange?.Invoke(Character.MaxHealth, CurrentHealth, -damage);
        CurrentHealth -= damage;

        if (_battleCharacterController.HasCharacterStartedMoving)
            DamageReceivedWhenWalking += damage;

        // don't shake on death
        if (CurrentHealth <= 0)
        {
            if (_lastAttacker != null)
                _lastAttacker.Character.GetExp(gameObject, true);
            await Die();
            return;
        }

        await ShakeOnDamageTaken();
    }

    public async Task ShakeOnDamageTaken()
    {
        // flash color
        // TODO: cache
        SpriteRenderer sr = _body.GetComponent<SpriteRenderer>();
        Color initialColor = sr.color;
        sr.DOColor(Color.black, 0.1f).SetLoops(4, LoopType.Yoyo).OnComplete(() => sr.color = initialColor);

        _audioManager.PlaySFX("Hurt", transform.position);

        // shake a character;
        float duration = 0.15f;
        float strength = 0.1f;

        DisableAILerp();
        _body.transform.DOShakePosition(duration, strength, 0, 0, false, true).SetLoops(2)
                       .OnComplete(() => EnableAILerp());

        await Task.Delay(500);
    }

    public void GetBuffed(GameObject attacker, Ability ability)
    {
        if (attacker.TryGetComponent(out CharacterStats stats))
            stats.Character.GetExp(gameObject);

        HandleModifier(ability);
        HandleStatus(attacker, ability);
    }

    void HandleModifier(Ability ability)
    {
        if (ability.StatModifier != null)
            AddModifier(ability);
    }

    async void HandleStatus(GameObject attacker, Ability ability)
    {
        if (ability.Status != null)
            await AddStatus(ability.Status, attacker);
    }

    public int CalculateAttackDir(Vector3 attackerPos)
    {
        Vector2 attackerFaceDir = (attackerPos - transform.position).normalized; // i swapped attacker pos with transform.pos
        Vector2 defenderFaceDir = _characterRendererManager.GetFaceDir();
        // side attack 1, face to face 2, from the back 0, 
        int attackDir = 1;
        if (attackerFaceDir + defenderFaceDir == Vector2.zero)
            attackDir = 2;
        if (attackerFaceDir + defenderFaceDir == attackerFaceDir * 2)
            attackDir = 0;

        return attackDir;
    }

    public int CalculateAttackDir(GameObject attacker)
    {
        Vector2 attackerFaceDir = attacker.GetComponentInChildren<CharacterRendererManager>().GetFaceDir(); // i swapped attacker pos with transform.pos
        Vector2 defenderFaceDir = _characterRendererManager.GetFaceDir();

        // side attack 1, face to face 2, from the back 0, 
        int attackDir = 1;
        if (attackerFaceDir + defenderFaceDir == Vector2.zero)
            attackDir = 2;
        if (attackerFaceDir + defenderFaceDir == attackerFaceDir * 2)
            attackDir = 0;

        return attackDir;
    }

    float CalculateDodgeChance(int attackDir, GameObject attacker, bool isRetaliation)
    {
        // retaliation is always face to face (which may not be true, but let's simplify for now)
        if (isRetaliation)
            attackDir = 2;

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
            if (a.WeaponType == WeaponType.Ranged)
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
        if (CalculateAttackDir(attacker) == 0)
            return false;

        // if attacker is yourself, don't retaliate
        if (attacker == gameObject)
            return false;

        return true;
    }

    public float GetDodgeChance(GameObject attacker, bool retaliation)
    {
        if (IsStunned)
            return 0;

        return CalculateDodgeChance(CalculateAttackDir(attacker), attacker, retaliation);
    }

    public void GainMana(int amount)
    {
        int manaBeforeChange = CurrentMana;
        CurrentMana += amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());
        OnManaChange?.Invoke(Character.MaxMana, manaBeforeChange, amount);
    }

    public void UseMana(int amount)
    {
        if (amount == 0)
            return;

        int manaBeforeChange = CurrentMana;

        CurrentMana -= amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());
        OnManaChange?.Invoke(Character.MaxMana, manaBeforeChange, -amount);
    }

    public void GainHealth(int healthGain, GameObject attacker, Ability ability)
    {
        if (attacker.TryGetComponent(out CharacterStats stats))
            stats.Character.GetExp(gameObject);

        healthGain = Mathf.Clamp(healthGain, 0, MaxHealth.GetValue() - CurrentHealth);
        OnHealthChange?.Invoke(Character.MaxHealth, CurrentHealth, healthGain);
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

        await MoveToPosition(_finalPos, 0.5f);
        await CheckCollision(ability);

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

        if (gameObject == null)
            return;

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

        EnableAILerp();
    }


    public async Task CheckCollision(Ability ability)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(_finalPos, 0.2f);
        foreach (Collider2D c in cols)
        {
            // player/enemy get damaged  and are moved back to their starting position
            // character colliders are children
            if (c.CompareTag(Tags.Player) || c.transform.gameObject.CompareTag(Tags.Enemy))
            {
                await CollideWithCharacter(ability, c);
                continue;
            }

            // character bounces back from being pushed into obstacle (and takes damage)
            if (c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.BoundCollider))
            {
                await CollideWithIndestructible(ability, c);
                continue;
            }

            // character destroys boulder when they are pushed into it
            if (c.CompareTag(Tags.PushableObstacle))
            {
                await CollideWithDestructible(ability, c);
                continue;
            }
        }
    }

    public async Task CollideWithCharacter(Ability ability, Collider2D col)
    {
        await TakeDamageFinal(ability.BasePower);

        CharacterStats targetStats = col.GetComponent<CharacterStats>();
        await targetStats.TakeDamageFinal(ability.BasePower);

        if (_tempObject != null)
            Destroy(_tempObject);

        if (CurrentHealth <= 0)
            return;
            
        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithIndestructible(Ability ability, Collider2D col)
    {
        await TakeDamageFinal(ability.BasePower);
        if (_tempObject != null)
            Destroy(_tempObject);

        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithDestructible(Ability ability, Collider2D col)
    {
        Destroy(col.transform.parent.gameObject); // TODO: call destroy self right?

        await TakeDamageFinal(ability.BasePower);
    }

    public async Task Die()
    {
        // playing death animation
        await _characterRendererManager.Die();
        GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
        effect.transform.DOLocalMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine);
        await transform.DOScale(Vector3.zero, 1f).AsyncWaitForCompletion();
        Destroy(effect);

        // kill all tweens TODO: is that OK?
        transform.DOKill();

        // movement script needs to clear the highlight 
        if (OnCharacterDeath != null)
            OnCharacterDeath(gameObject);

        // die in some way
        // this method is meant to be overwirtten
        Destroy(gameObject);
    }

    public void SetAttacker(bool isAttacker) { IsAttacker = isAttacker; }

    void AddModifier(Ability ability)
    {
        foreach (Stat s in Stats)
            if (s.Type == ability.StatModifier.StatType)
            {
                StatModifier mod = Instantiate(ability.StatModifier);
                mod.Initialize(gameObject);
                s.AddModifier(mod); // stat checks if modifier is a dupe, to prevent stacking
                OnModifierAdded?.Invoke(mod);
            }
    }

    public override async Task<Status> AddStatus(Status s, GameObject attacker)
    {
        Status addedStatus = await base.AddStatus(s, attacker);
        if (_battleCharacterController.HasCharacterStartedMoving)
            _statusesAddedWhenWalking.Add(addedStatus);

        // noone cares?
        return addedStatus;
    }

    public void ResolveGoingBack()
    {
        CurrentHealth += DamageReceivedWhenWalking;
        DamageReceivedWhenWalking = 0;

        foreach (Status s in _statusesAddedWhenWalking)
            RemoveStatus(s);
        _statusesAddedWhenWalking.Clear();
        foreach (Status s in _statusesBeforeWalking)
            AddStatusWithoutTrigger(s, s.Attacker);
    }

    protected void DisableAILerp()
    {
        _aiLerp.isStopped = true;
    }

    protected void EnableAILerp()
    {
        _aiLerp.isStopped = false;
    }

    void DisableAiLerpSearch()
    {
        _aiLerp.canSearch = false;
    }

    public void ReplaceAbility(Ability ability)
    {
        _replacedAbility = Abilities[0];

        Ability clone = Instantiate(ability);
        clone.Initialize(gameObject);
        Abilities[0] = clone;

        OnAbilityAdded?.Invoke(ability);
    }

    public void RevertAbilityReplace()
    {
        Abilities[0] = _replacedAbility;
        _replacedAbility = null;
        OnAbilityAdded?.Invoke(Abilities[0]);
    }
}

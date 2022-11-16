using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UIElements;
public class CharacterStats : BaseStats, IHealable<GameObject, Ability>, IAttackable<GameObject, Ability>, IPushable<Vector3, GameObject, Ability>, IBuffable<GameObject, Ability>
{
    // global
    BattleCharacterController _battleCharacterController;
    BattleUI _battleUI;
    AudioManager _audioManager;

    // local
    CharacterRendererManager _characterRendererManager;
    AILerp _aiLerp;

    [HideInInspector] public Character Character { get; private set; }

    // Stats are accessed by other scripts need to be public.
    // TODO: I could make them private and use only list to get info about stats
    [HideInInspector] public Stat Power = new();
    [HideInInspector] public Stat MaxHealth = new();
    [HideInInspector] public Stat MaxMana = new();
    [HideInInspector] public Stat Armor = new();
    [HideInInspector] public Stat MovementRange = new();

    // lists of abilities
    [Header("Filled on character init")]
    [HideInInspector] public List<Stat> Stats = new();
    [HideInInspector] public List<Ability> Abilities = new();
    [HideInInspector] public AttackAbility BasicAttack;

    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }

    // retaliation on interaction
    public bool IsAttacker { get; private set; }
    bool _hasRetaliatedThisTurn;

    // pushable variables
    Vector3 _startingPos;
    Vector3 _finalPos;
    GameObject _tempObject;

    // dmg
    [SerializeField] GameObject _deathEffect;
    [SerializeField] GameObject _body;
    protected SpriteRenderer _bodySpriteRenderer;
    Color _initialBodyColor;
    Vector3 _initialBodyPosition;

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
    public event Action OnCharacterInitialized;
    public event Action<GameObject> OnCharacterDeath;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnManaChanged;
    public event Action<StatModifier> OnModifierAdded;
    public event Action<Ability> OnAbilityAdded;

    protected override void Awake()
    {
        base.Awake();
        _battleCharacterController = BattleCharacterController.Instance;
        _battleUI = BattleUI.Instance;
        _audioManager = AudioManager.Instance;

        // local
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        _bodySpriteRenderer = _body.GetComponent<SpriteRenderer>();
        _initialBodyColor = _bodySpriteRenderer.color;
        _initialBodyPosition = _body.transform.localPosition;
        _aiLerp = GetComponent<AILerp>();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        AddStatsToList();
    }

    protected virtual async void TurnManager_OnBattleStateChanged(BattleState state)
    {
        await Task.Yield();
    }

    protected async Task ResolveStatuses()
    {
        for (int i = Statuses.Count - 1; i >= 0; i--)
        {
            if (Statuses[i].ShouldTrigger())
            {
                await Statuses[i].TriggerStatus();
                if (CurrentHealth <= 0)
                    return;
            }
        }

        if (TurnManager.CurrentTurn <= 1)
            return;

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
        InitializeCharacter();
        OnCharacterInitialized?.Invoke();
    }

    void InitializeCharacter()
    {
        Character.ResolveItems();

        // taking values from scriptable object to game
        Power.Initialize(StatType.Power, Character.GetStatValue("Power"), Character);
        MaxHealth.Initialize(StatType.MaxHealth, Character.GetStatValue("MaxHealth"), Character);
        MaxMana.Initialize(StatType.MaxMana, Character.GetStatValue("MaxMana"), Character);
        Armor.Initialize(StatType.Armor, Character.GetStatValue("Armor"), Character);
        MovementRange.Initialize(StatType.MovementRange, Character.GetStatValue("MovementRange"), Character);

        CurrentHealth = MaxHealth.GetValue();
        CurrentMana = MaxMana.GetValue();

        // adding basic attack from weapon to basic abilities to be instantiated
        if (Character.Weapon != null)
        {
            // set weapon for animations & deactivate the game object
            WeaponHolder wh = GetComponentInChildren<WeaponHolder>();
            wh.SetWeapon(Character.Weapon);
            wh.gameObject.SetActive(false);

            BasicAttack = (AttackAbility)Instantiate(Character.Weapon.BasicAttack);
            BasicAttack.Initialize(gameObject);
        }


        foreach (Ability ability in Character.Abilities)
        {
            if (ability == null)
                continue;

            var clone = Instantiate(ability);
            Abilities.Add(clone);
            clone.Initialize(gameObject);
        }

        foreach (Item item in Character.Items)
        {
            var clone = Instantiate(item);
            // hmmm... i probably need to clone it now.
            clone.Initialize(this);
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

        if (IsShielded)
        {
            ShieldDamage();
            return wasAttackSuccesful;
        }

        wasAttackSuccesful = true;
        _lastAttacker = attacker.GetComponent<CharacterStats>();
        HandleModifier(ability);
        await TakeDamageFinal(damage);

        if (CurrentHealth <= 0) // for safety
            return wasAttackSuccesful;

        await HandleStatus(attacker, ability);

        if (CurrentHealth <= 0) // for safety
            return wasAttackSuccesful;

        // in the side 1, face to face 2, from the back 0, 
        int attackDir = CalculateAttackDir(attacker.gameObject.transform.position);
        if (attackDir == 0)
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

        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{Character.name} retaliates."), BattleLogLineType.Damage));
        _hasRetaliatedThisTurn = true;
        return wasAttackSuccesful;
    }

    void ShieldDamage()
    {
        ShieldStatus shieldStatus = null;
        foreach (Status s in Statuses)
            if (s is ShieldStatus)
                shieldStatus = s as ShieldStatus;

        if (shieldStatus != null)
            RemoveStatus(shieldStatus);

        _damageUI.DisplayOnCharacter("Shielded!", 24, Color.magenta);
    }

    public async Task TakeDamageFinal(int damage, bool shake = true)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // displaying damage UI
        _damageUI.DisplayOnCharacter(damage.ToString(), 36, Helpers.GetColor("damageRed"));

        OnHealthChanged?.Invoke(-damage);
        CurrentHealth -= damage;

        if (_battleCharacterController.HasCharacterStartedMoving)
            DamageReceivedWhenWalking += damage;

        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{Character.CharacterName} takes {damage} damage."), BattleLogLineType.Damage));
        // don't shake on death
        if (CurrentHealth <= 0)
        {
            await Die();
            return;
        }
        if (shake)
            await ShakeOnDamageTaken();

        _body.transform.localPosition = _initialBodyPosition;
    }

    public async Task ShakeOnDamageTaken()
    {
        // flash color
        _bodySpriteRenderer.DOColor(Color.black, 0.1f).SetLoops(4, LoopType.Yoyo).OnComplete(HandleBodyColor);

        _audioManager.PlaySFX("Hurt", transform.position);

        // shake a character;
        float duration = 0.15f;
        float strength = 0.1f;

        DisableAILerp();
        _body.transform.DOShakePosition(duration, strength, 0, 0, false, true).SetLoops(2)
                       .OnComplete(() => EnableAILerp());

        await Task.Delay(500);
    }

    protected virtual void HandleBodyColor()
    {
        _bodySpriteRenderer.color = _initialBodyColor;
    }

    public async void GetBuffed(GameObject attacker, Ability ability)
    {
        HandleModifier(ability);
        await HandleStatus(attacker, ability);
    }

    void HandleModifier(Ability ability)
    {
        if (ability.StatModifier != null)
            AddModifier(ability);
    }

    async Task HandleStatus(GameObject attacker, Ability ability)
    {
        if (ability.Status != null)
            await AddStatus(ability.Status, attacker);
    }

    public int CalculateAttackDir(Vector3 attackerPos)
    {
        Vector2 attackerDir = (attackerPos - transform.position).normalized; // i swapped attacker pos with transform.pos
        return BaseCalculateAttackDir(attackerDir);
    }

    int BaseCalculateAttackDir(Vector2 attackerDir)
    {
        Vector2 defenderFaceDir = _characterRendererManager.GetFaceDir();

        // let's do it with a dot product
        float dot = Vector2.Dot(defenderFaceDir, attackerDir);

        // https://twitter.com/freyaholmer/status/1200807790580768768?lang=en
        // side attack 1, face to face 2, from the back 0, 
        int attackDir = 1;
        if (dot > 0.75f)
            attackDir = 2;
        if (dot < -0.75f)
            attackDir = 0;

        return attackDir;
    }


    public AttackAbility GetRetaliationAbility()
    {
        // no retaliation with ranged attacks 
        if (BasicAttack.WeaponType == WeaponType.Ranged)
            return null;

        return (AttackAbility)BasicAttack;
    }

    // used by info card ui
    public bool WillRetaliate(GameObject attacker)
    {
        if (IsStunned)
            return false;

        if (_hasRetaliatedThisTurn)
            return false;

        // if attacked from the back, don't retaliate
        if (CalculateAttackDir(attacker.transform.position) == 0)
            return false;

        // if attacker is yourself, don't retaliate
        if (attacker == gameObject)
            return false;

        return true;
    }

    public void GainMana(int amount)
    {
        int manaBeforeChange = CurrentMana;
        CurrentMana += amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());
        OnManaChanged?.Invoke(amount);
    }

    public void UseMana(int amount)
    {
        if (amount == 0)
            return;

        int manaBeforeChange = CurrentMana;

        CurrentMana -= amount;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana.GetValue());
        OnManaChanged?.Invoke(-amount);
    }

    public async void GainHealth(int healthGain, GameObject attacker, Ability ability)
    {
        healthGain = Mathf.Clamp(healthGain, 0, MaxHealth.GetValue() - CurrentHealth);
        OnHealthChanged?.Invoke(healthGain);
        CurrentHealth += healthGain;

        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{Character.CharacterName} gains {healthGain} health."), BattleLogLineType.Damage));
        _damageUI.DisplayOnCharacter(healthGain.ToString(), 36, Helpers.GetColor("healthGainGreen"));

        if (ability == null)
            return;

        HandleModifier(ability);
        await HandleStatus(attacker, ability);
    }

    public async Task GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{Character.CharacterName} is pushed."), BattleLogLineType.Damage));

        _startingPos = transform.position;
        _finalPos = transform.position + dir;

        HandleModifier(ability);
        await HandleStatus(attacker, ability);

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
        await targetStats.TakeDamageFinal(ability.BasePower, false);

        if (_tempObject != null)
            Destroy(_tempObject);

        if (CurrentHealth <= 0)
            return;

        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithIndestructible(Ability ability, Collider2D col)
    {
        await TakeDamageFinal(ability.BasePower, false);
        if (_tempObject != null)
            Destroy(_tempObject);

        await MoveToPosition(_startingPos, 0.5f);
    }

    public async Task CollideWithDestructible(Ability ability, Collider2D col)
    {
        // https://stackoverflow.com/questions/22629951/suppressing-warning-cs4014-because-this-call-is-not-awaited-execution-of-the
        _ = col.GetComponent<IDestroyable>().DestroySelf();
        await TakeDamageFinal(ability.BasePower, false);
    }

    public async Task Die()
    {
        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{Character.CharacterName} dies."), BattleLogLineType.Death));

        // playing death animation
        await _characterRendererManager.Die();
        Vector3 offset = new Vector3(0f, 0.5f, 0f);
        GameObject effect = Instantiate(_deathEffect, transform.position + offset, Quaternion.identity);
        effect.transform.DOLocalMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine);
        await transform.DOScale(Vector3.zero, 1f).AsyncWaitForCompletion();
        Destroy(effect);

        // kill all tweens
        transform.DOKill();

        // movement script needs to clear the highlight 
        if (OnCharacterDeath != null)
            OnCharacterDeath(gameObject);

        gameObject.SetActive(false);
        Invoke("DestroySelf", 3);
    }

    void DestroySelf() { Destroy(gameObject); }

    public void SetAttacker(bool isAttacker) { IsAttacker = isAttacker; }

    void AddModifier(Ability ability)
    {
        foreach (Stat s in Stats)
            if (s.Type == ability.StatModifier.StatType)
            {
                StatModifier mod = Instantiate(ability.StatModifier);
                mod.Initialize(gameObject);
                bool isAdded = s.AddModifier(mod); // stat checks if modifier is a dupe, to prevent stacking
                if (isAdded)
                {
                    OnModifierAdded?.Invoke(mod);
                    VisualElement el = new();
                    el.style.flexDirection = FlexDirection.Row;
                    el.Add(new Label($"{Character.CharacterName} gets "));
                    el.Add(new ModifierVisual(mod));
                    _battleUI.DisplayBattleLog(new BattleLogLine(el, BattleLogLineType.Status));
                }
            }
    }

    public override async Task<Status> AddStatus(Status s, GameObject attacker, bool trigger = true)
    {
        Status addedStatus = await base.AddStatus(s, attacker, trigger);
        if (_battleCharacterController.HasCharacterStartedMoving)
            _statusesAddedWhenWalking.Add(addedStatus);

        VisualElement el = new();
        el.style.flexDirection = FlexDirection.Row;
        el.Add(new Label($"{Character.CharacterName} gets "));
        el.Add(new ModifierVisual(s));
        _battleUI.DisplayBattleLog(new BattleLogLine(el, BattleLogLineType.Status));

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

    protected async Task HandleYourTeamTurn()
    {
        await ResolveStatuses();
        _hasRetaliatedThisTurn = false;
    }

    protected void HandleOppositTeamTurn()
    {
        ResolveModifiersTurnEnd();
    }

}

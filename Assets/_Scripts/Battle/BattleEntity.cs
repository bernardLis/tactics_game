using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using System.Linq;

public class BattleEntity : MonoBehaviour
{
    [SerializeField] GameObject _projectileSpawnPoint;

    public Collider Collider { get; private set; }

    bool _isPlayer;
    GameObject _GFX;
    Material _material;
    Texture2D _emissionTexture;
    Animator _animator;

    List<BattleEntity> _opponentList = new();

    public ArmyEntity ArmyEntity { get; private set; }
    public float CurrentHealth { get; private set; }

    BattleEntity _opponent;
    NavMeshAgent _agent;

    float _currentAttackCooldown;

    public int KilledEnemiesCount { get; private set; }

    bool _isSpawned;
    bool _gettingHit;
    bool _isGrabbed;
    public bool IsDead { get; private set; }

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;
    IEnumerator _attackCoroutine;

    public event Action<float> OnHealthChanged;
    public event Action<int> OnEnemyKilled;
    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();
    }

    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;

        /* ANIMATOR UPDATE */
        if (!_agent.isActiveAndEnabled || _agent.isStopped)
            _animator.SetBool("Move", false);
    }

    public void Initialize(bool isPlayer, ArmyEntity stats, ref List<BattleEntity> opponents)
    {
        Collider = GetComponent<Collider>();

        _animator = GetComponentInChildren<Animator>();
        _GFX = _animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");

        _isPlayer = isPlayer;
        if (!isPlayer)
        {
            _material.SetTexture("_EmissionMap", null);
            _material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 0.2f));

            _material.SetFloat("_Metallic", 0.5f);
        }

        ArmyEntity = stats;
        CurrentHealth = stats.Health;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = stats.AttackRange;
        _agent.speed = stats.Speed;
        _agent.enabled = true;

        _opponentList = opponents;

        StartRunEntityCoroutine();
    }

    public void StopRunEntityCoroutine()
    {
        Debug.Log($"stop");
        _agent.enabled = false;
        StopCoroutine(_runEntityCoroutine);
        _runEntityCoroutine = null;
    }

    public void StartRunEntityCoroutine()
    {
        Debug.Log($"start");
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    IEnumerator RunEntity()
    {
        Debug.Log($"run entity started");
        if (!_isSpawned)
        {
            _isSpawned = true;
            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        }

        // HERE: something smarter
        while (!IsDead)
        {
            if (_opponentList.Count == 0)
            {
                yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
                Celebrate();
                yield break;
            }

            if (_opponent == null || _opponent.IsDead)
                ChooseNewTarget();

            _agent.enabled = true;
            _agent.destination = _opponent.transform.position;
            // TODO: get rid of this somehow...
            // yield return new WaitForSeconds(Random.Range(0.2f, 0.6f)); // otherwise agent does not move
            _animator.SetBool("Move", true);

            // path to target
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
            {
                if (_opponent == null) yield break;
                if (IsDead) yield break;
                _agent.destination = _opponent.transform.position;
                yield return null;
            }

            // reached destination
            _agent.enabled = false;

            // if (_attackCoroutine != null) yield break;
            Debug.Log($"before attack coroutine");
            if (ArmyEntity.Projectile == null)
                _attackCoroutine = Attack();
            else
                _attackCoroutine = Shoot();

            yield return StartCoroutine(_attackCoroutine);
        }
    }

    void ChooseNewTarget()
    {
        // choose a random opponent with a bias towards closer opponents
        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _opponentList)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            distances.Add(be, distance);
        }

        if (distances.Count == 0) return;

        var closest = distances.OrderByDescending(pair => pair.Value).Reverse().Take(10);
        float v = Random.value;

        //https://stats.stackexchange.com/questions/277298/create-a-higher-probability-to-smaller-values
        Dictionary<BattleEntity, float> closestBiased = new();
        // this number decides bias towards closer opponents
        float e = 0.91f; // range 0.9 - 0.99 I think
        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            float value = Mathf.Pow(e, entry.Value);
            closestBiased.Add(entry.Key, value);
            sum += value;
        }

        Dictionary<BattleEntity, float> closestNormalized = new();
        foreach (KeyValuePair<BattleEntity, float> entry in closestBiased)
            closestNormalized.Add(entry.Key, entry.Value / sum);

        foreach (KeyValuePair<BattleEntity, float> entry in closestNormalized)
        {
            if (v < entry.Value)
            {
                _opponent = entry.Key;
                return;
            }
            v -= entry.Value;
        }

        // should never get here...
        _opponent = _opponentList[Random.Range(0, _opponentList.Count)];
    }

    IEnumerator Attack()
    {
        Debug.Log($"attack before checks");
        if (!CanAttack()) yield break;
        if (!IsOpponentInRange()) yield break;
        Debug.Log($"attack after checks");

        _animator.SetTrigger("Attack");

        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
        GameObject hitInstance = Instantiate(ArmyEntity.HitPrefab, _opponent.Collider.bounds.center, q);

        yield return _opponent.GetHit(this);

        Destroy(hitInstance);
        _attackCoroutine = null;
    }


    IEnumerator Shoot()
    {
        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange()) yield break;

        Debug.Log($"shoot");
        GameObject projectileInstance = Instantiate(ArmyEntity.Projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.LookAt(_opponent.transform);
        projectileInstance.transform.parent = _GFX.transform;

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        _animator.SetTrigger("Attack");
        // HERE: projectile spawning and animation delay per entity
        yield return new WaitForSeconds(0.2f);
        // yield return new WaitWhile(
        //     () => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= Stats.ProjectileSpawnAnimationDelay);

        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        // spawn projectile

        // HERE: projectile speed
        projectileInstance.GetComponent<Projectile>().Shoot(this, _opponent, 20, ArmyEntity.Power);
        _attackCoroutine = null;
    }

    bool CanAttack()
    {
        if (_gettingHit) return false;
        return _currentAttackCooldown < 0;
    }

    bool IsOpponentInRange()
    {
        if (_opponent == null) return false;
        if (_opponent.IsDead) return false;

        // +0.5 wiggle room
        return Vector3.Distance(transform.position, _opponent.transform.position) < ArmyEntity.AttackRange + 0.5f;
    }

    public float GetTotalHealth() { return ArmyEntity.Health; }
    public float GetCurrentHealth() { return CurrentHealth; }

    public int GetHealed(Ability ability)
    {
        float value = ability.GetPower();
        CurrentHealth += value;
        if (CurrentHealth > ArmyEntity.Health)
            CurrentHealth = ArmyEntity.Health;

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = value.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetFloatingTextGradient(ability.Element.Color);
        _feelPlayer.PlayFeedbacks(transform.position);

        OnHealthChanged?.Invoke(CurrentHealth);

        return Mathf.RoundToInt(value);
    }

    public IEnumerator GetHit(BattleEntity attacker, Ability ability = null)
    {
        Debug.Log($"get hit");
        if (IsDead) yield break;
        StopRunEntityCoroutine();

        float dmg = 0;
        Color dmgColor = Color.white;
        if (ability != null)
        {
            dmg = ArmyEntity.CalculateDamage(ability);
            dmgColor = ability.Element.Color;
        }
        if (attacker != null)
        {
            dmg = ArmyEntity.CalculateDamage(attacker);
            dmgColor = attacker.ArmyEntity.Element.Color;
        }

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = dmg.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetFloatingTextGradient(dmgColor);
        _feelPlayer.PlayFeedbacks(transform.position);

        _gettingHit = true;
        CurrentHealth -= dmg;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            if (attacker != null) attacker.IncreaseKillCount();
            yield return Die(attacker, ability);
            yield break;
        }

        _animator.SetTrigger("Take Damage");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        _gettingHit = false;

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public bool CanBeGrabbed()
    {
        if (IsDead) return false;
        return true;
    }

    public void Grabbed()
    {
        _opponent = null;
        _isGrabbed = true;
        StopRunEntityCoroutine();
        _animator.enabled = false;
    }

    public void Released()
    {
        _isGrabbed = false;
        _animator.enabled = true;
        StartRunEntityCoroutine();
    }

    void Celebrate()
    {
        transform.DODynamicLookAt(Camera.main.transform.position, 0.2f);
        _animator.SetBool("Celebrate", true);
    }

    public IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        _animator.SetBool("Celebrate", false);

        IsDead = true;
        _animator.SetTrigger("Die");
        OnDeath?.Invoke(this, attacker, ability);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        ToggleHighlight(false);
    }

    public void IncreaseKillCount()
    {
        KilledEnemiesCount++;
        OnEnemyKilled?.Invoke(KilledEnemiesCount);
    }

    public void ToggleHighlight(bool isOn)
    {
        if (!isOn)
        {
            TurnHighlightOff();
            return;
        }

        if (IsDead) return;
        TurnHighlightOn();
    }

    void TurnHighlightOff()
    {
        if (_emissionTexture != null && _isPlayer)
        {
            _material.SetTexture("_EmissionMap", _emissionTexture);
            _material.SetColor("_EmissionColor", Color.black);
            return;
        }

        if (_isPlayer)
            _material.SetColor("_EmissionColor", Color.black);
        else
            _material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 0.2f));
    }

    void TurnHighlightOn()
    {
        _material.SetTexture("_EmissionMap", null);

        if (_isPlayer)
            _material.SetColor("_EmissionColor", Color.blue);
        else
            _material.SetColor("_EmissionColor", Color.red);
    }

    public Gradient GetFloatingTextGradient(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.5f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 1f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.5f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }

}

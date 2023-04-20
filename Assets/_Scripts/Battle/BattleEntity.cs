using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class BattleEntity : MonoBehaviour
{
    [SerializeField] Sound _hurtSound;
    [SerializeField] Image _elementImage;

    List<BattleEntity> _opponentList = new();

    public GameObject GFX;
    Material _originalMaterial;
    Material _gfxMaterial;

    const string _tweenHighlightId = "_tweenHighlightId";

    public ArmyEntity Stats { get; private set; }
    public Hero Hero { get; private set; }
    public float CurrentHealth { get; private set; }

    BattleEntity _opponent;
    NavMeshAgent _agent;

    float _currentAttackCooldown;

    public int KilledEnemiesCount { get; private set; }

    bool _gettingHit;
    public bool IsDead { get; private set; }

    MMF_Player _feelPlayer;

    public event Action<float> OnHealthChanged;
    public event Action<BattleEntity> OnDeath;

    IEnumerator _runEntityCoroutine;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();
    }

    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
    }

    public void Initialize(Material mat, Hero hero, ArmyEntity stats, ref List<BattleEntity> opponents)
    {
        if (hero != null)
            Hero = hero;
        Stats = stats;
        CurrentHealth = stats.Health;

        _originalMaterial = mat;
        GFX.GetComponent<MeshRenderer>().material = _originalMaterial;
        _gfxMaterial = GFX.GetComponent<MeshRenderer>().material;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = stats.Speed + hero.Speed.GetValue();
        _agent.stoppingDistance = stats.AttackRange;

        _elementImage.sprite = Stats.Element.Icon;

        _opponentList = opponents;

        StartRunEntityCoroutine();
    }

    public void StopRunEntityCoroutine()
    {
        _agent.enabled = false;
        StopCoroutine(_runEntityCoroutine);
    }

    public void StartRunEntityCoroutine()
    {
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    IEnumerator RunEntity()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f)); // random delay at the beginning
        while (!IsDead)
        {
            if (_opponentList.Count == 0)
            {
                Celebrate();
                yield break;
            }
            if (_opponent == null || _opponent.IsDead)
                ChooseNewTarget();

            _agent.enabled = true;
            _agent.destination = _opponent.transform.position;
            yield return new WaitForSeconds(0.2f);

            // path to target
            while (_agent.remainingDistance > _agent.stoppingDistance)
            {
                if (_opponent == null) break;
                if (IsDead) break;
                _agent.destination = _opponent.transform.position;
                transform.LookAt(_opponent.transform);
                yield return null;
            }

            // reached destination
            _agent.enabled = false;


            // HERE: ability testing
            /*
                        // HERE: something smarter
                        if (Stats.Projectile == null)
                            yield return StartCoroutine(Attack());
                        else
                            yield return StartCoroutine(Shoot());
                            */
        }
    }

    void ChooseNewTarget()
    {
        _opponent = _opponentList[Random.Range(0, _opponentList.Count)];
    }

    IEnumerator Attack()
    {
        while (_currentAttackCooldown > 0) yield return null;
        while (_gettingHit) yield return null;
        if (_opponent == null || _opponent.IsDead) yield break;
        if (Vector3.Distance(transform.position, _opponent.transform.position) > Stats.AttackRange + 0.5f) // +0.5 wiggle room
            yield break; // target ran away

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        Vector3 punchRotation = new(45f, 0f, 0f);
        yield return GFX.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        GameObject hitInstance = Instantiate(Stats.HitPrefab, _opponent.transform.position, Quaternion.identity);

        _currentAttackCooldown = Stats.AttackCooldown;

        yield return _opponent.GetHit(this);

        Destroy(hitInstance);
    }

    IEnumerator Shoot()
    {
        while (_currentAttackCooldown > 0) yield return null;
        while (_gettingHit) yield return null;
        if (_opponent == null || _opponent.IsDead) yield break;
        if (Vector3.Distance(transform.position, _opponent.transform.position) > Stats.AttackRange)
            yield break; // target ran away

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        Vector3 punchRotation = new(45f, 0f, 0f);
        GFX.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        _currentAttackCooldown = Stats.AttackCooldown;

        // spawn projectile
        GameObject projectileInstance = Instantiate(Stats.Projectile, GFX.transform.position, Quaternion.identity);
        projectileInstance.transform.LookAt(_opponent.transform);

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Shoot(this, _opponent, 20, Stats.Power);
    }

    public float GetTotalHealth() { return Stats.Health; }
    public float GetCurrentHealth() { return CurrentHealth; }

    public void GetHealed(Ability ability)
    {
        float value = ability.GetPower();
        CurrentHealth += value;
        if (CurrentHealth > Stats.Health)
            CurrentHealth = Stats.Health;

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = value.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetDamageTextGradient(ability.Element.Color);
        _feelPlayer.PlayFeedbacks(transform.position);

        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public IEnumerator GetHit(BattleEntity attacker, Ability ability = null)
    {
        if (IsDead) yield break;

        float dmg = 0;
        Color dmgColor = Color.white;
        if (ability != null)
        {
            dmg = Stats.CalculateDamage(ability);
            dmgColor = ability.Element.Color;
        }
        if (attacker != null)
        {
            dmg = Stats.CalculateDamage(attacker);
            dmgColor = attacker.Stats.Element.Color;
        }

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = dmg.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetDamageTextGradient(dmgColor);
        _feelPlayer.PlayFeedbacks(transform.position);

        _gettingHit = true;
        CurrentHealth -= dmg;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            if (attacker != null) attacker.IncreaseKillCount();
            yield return Die();
            yield break;
        }
        //      if (Random.value > 0.5f)
        //         AudioManager.Instance.PlaySFX(_hurtSound, transform.position);
        yield return transform.DOShakePosition(0.2f, 0.5f).WaitForCompletion();
        _gettingHit = false;
    }

    public Gradient GetDamageTextGradient(Color color)
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

    void Celebrate()
    {
        transform.LookAt(Camera.main.transform);
        transform.DOJump(transform.position + Vector3.up * Random.Range(0.2f, 1.5f), 1, 1, 1f)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetDelay(Random.Range(0.5f, 1f));
    }

    public IEnumerator Die()
    {
        IsDead = true;
        OnDeath?.Invoke(this);
        yield return new WaitForSeconds(0.2f);
        transform.DORotate(new Vector3(-90, 0, 0), 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        yield return transform.DOMoveY(0, 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        ToggleHighlight(false);
    }

    public void IncreaseKillCount() { KilledEnemiesCount++; }

    public void ToggleHighlight(bool isOn)
    {
        if (IsDead) return;

        if (isOn)
        {
            _gfxMaterial.DOColor(Color.white, 0.2f).SetLoops(-1, LoopType.Yoyo).SetId(_tweenHighlightId);
        }
        else
        {
            DOTween.Kill(_tweenHighlightId);
            _gfxMaterial.color = _originalMaterial.color;
        }
    }
}

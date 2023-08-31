using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Shapes;

public class BattleTurret : MonoBehaviour, IGrabbable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;
    BattleGrabManager _grabManager;

    [SerializeField] GameObject _rangeIndicator;
    Disc _rangeDisc;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] GameObject _GFX;

    public int Team { get; private set; }
    public Turret Turret { get; private set; }

    BattleEntity _target;

    IEnumerator _runTurretCoroutine;

    bool _isTooltipActive;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _grabManager = BattleGrabManager.Instance;
    }

    public void Initialize(Turret turret)
    {
        Team = 0; // HERE: turret team

        Turret = turret;
        Turret.OnTurretUpgradePurchased += UpdateTurret;
        UpdateTurret();

        _rangeIndicator.SetActive(true);
    }

    void UpdateTurret()
    {
        UpdateGFX();
        UpdateRangeIndicator();
    }

    public void UpdateGFX()
    {
        // TODO: maybe some effect
        if (_GFX != null) Destroy(_GFX);
        _GFX = Instantiate(Turret.GetCurrentUpgrade().GFXPrefab, transform.position + Vector3.up, Quaternion.identity);
        _GFX.transform.parent = transform;
    }

    void UpdateRangeIndicator()
    {
        _rangeDisc = _rangeIndicator.GetComponent<Disc>();
        _rangeDisc.Radius = Turret.GetCurrentUpgrade().Range;

        Color c = GameManager.PlayerTeamColor;
        c.a = 0.3f;
        _rangeDisc.Color = c;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_runTurretCoroutine == null) return;

        _rangeIndicator.SetActive(true);
        _tooltipManager.ShowInfo($"Click for turret upgrades");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_runTurretCoroutine == null) return;

        if (!_isTooltipActive)
            _rangeIndicator.SetActive(false);

        _tooltipManager.HideInfo();
    }

    public void StartTurretCoroutine()
    {
        _rangeIndicator.SetActive(false);

        _runTurretCoroutine = RunTurret();
        StartCoroutine(_runTurretCoroutine);
    }

    IEnumerator RunTurret()
    {
        while (true)
        {
            yield return new WaitForSeconds(Turret.GetCurrentUpgrade().RateOfFire);
            ChooseNewTarget();
            if (_target == null) continue;
            FireProjectile();
        }
    }

    void ChooseNewTarget()
    {
        _target = null;

        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _battleManager.OpponentEntities)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            if (distance <= Turret.GetCurrentUpgrade().Range)
                distances.Add(be, distance);
        }

        if (distances.Count == 0) return;

        _target = distances.OrderByDescending(pair => pair.Value).Reverse().Take(1).First().Key;
    }

    void FireProjectile()
    {
        Projectile projectileInstance = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectileInstance.transform.parent = transform;
        projectileInstance.Shoot(this, _target, Turret.GetCurrentUpgrade().Power);
    }

    public bool CanBeGrabbed() { return true; }

    public void Grabbed()
    {
        if (_runTurretCoroutine != null)
            StopCoroutine(_runTurretCoroutine);
    }

    public void Released()
    {
        if (_runTurretCoroutine != null) StopCoroutine(_runTurretCoroutine);
        StartCoroutine(_runTurretCoroutine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_grabManager.TryGrabbing(gameObject)) return;

        if (_runTurretCoroutine == null) return;
        _isTooltipActive = true;

        if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return;
        TurretCard c = new(Turret);
        _tooltipManager.ShowTooltip(c, gameObject);

        _rangeIndicator.SetActive(true);

        c.OnShowTurretUpgrade += () => _rangeDisc.Radius = Turret.GetNextUpgrade().Range;
        c.OnHideTurretUpgrade += () => _rangeDisc.Radius = Turret.GetCurrentUpgrade().Range;

        _tooltipManager.OnTooltipHidden += OnTooltipHidden;

    }

    void OnTooltipHidden()
    {
        _isTooltipActive = false;
        _rangeIndicator.SetActive(false);
        _tooltipManager.OnTooltipHidden -= OnTooltipHidden;
    }
}

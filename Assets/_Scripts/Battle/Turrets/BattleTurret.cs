using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Shapes;

public class BattleTurret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;


    [SerializeField] GameObject _rangeIndicator;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] GameObject _GFX;

    public int Team { get; private set; }
    public Turret Turret { get; private set; }

    BattleEntity _target;

    IEnumerator _runTurretCoroutine;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
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
        Disc d = _rangeIndicator.GetComponent<Disc>();
        d.Radius = Turret.GetCurrentUpgrade().Range;

        Color c = GameManager.PlayerTeamColor;
        c.a = 0.3f;
        d.Color = c;
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
        // TODO: turret upgrades
        // display a turret card 
        // with turret stats
        // and a button to upgrade the turret
        // subscribe to the button's OnClick event
        if (_runTurretCoroutine == null) return;

        Debug.Log($"turret click");
        _tooltipManager.DisplayTooltip(Turret);

        /*
        if (_battleGrabManager.IsGrabbingEnabled) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _storeyTurretElement = new(_storeyTurret, _battleTurret);
        _storeyTurretElement.style.opacity = 0;
        _battleManager.Root.Add(_storeyTurretElement);
        _battleManager.PauseGame();
        DOTween.To(x => _storeyTurretElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _storeyTurretElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_storeyTurretElement);
            _storeyTurretElement = null;
        };
        */
    }
}

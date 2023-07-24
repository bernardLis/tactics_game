using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Shapes;

public class BattleTurret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BattleManager _battleManager;

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
    }

    public void Initialize(Turret turret)
    {
        Turret = turret;
        Turret.OnTurretUpgradePurchased += UpdateGFX;
        UpdateGFX();
        Team = 0; // HERE: turret team

        _runTurretCoroutine = RunTurret();
        StartCoroutine(_runTurretCoroutine);
    }

    public void UpdateGFX()
    {
        // TODO: maybe some effect
        if (_GFX != null) Destroy(_GFX);
        _GFX = Instantiate(Turret.GetCurrentUpgrade().GFXPrefab, transform.position + Vector3.up, Quaternion.identity);
        _GFX.transform.parent = transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Disc d = _rangeIndicator.GetComponent<Disc>();
        d.Radius = Turret.GetCurrentUpgrade().Range;
        
        Color c = GameManager.PlayerTeamColor;
        c.a = 0.3f;
        d.Color = c;

        _rangeIndicator.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rangeIndicator.SetActive(false);
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
        // choose a random opponent with a bias towards closer opponents
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

}

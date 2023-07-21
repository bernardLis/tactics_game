using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BattleTurret : MonoBehaviour
{
    BattleManager _battleManager;

    BattleEntity _target;
    public float Range = 20f;
    [SerializeField] float _fireRate = 1f;
    [SerializeField] Projectile _projectilePrefab;

    IEnumerator _runTurretCoroutine;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _runTurretCoroutine = RunTurret();
        StartCoroutine(_runTurretCoroutine);
    }

    IEnumerator RunTurret()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fireRate);
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
            if (distance <= Range)
                distances.Add(be, distance);
        }

        if (distances.Count == 0) return;

        _target = distances.OrderByDescending(pair => pair.Value).Reverse().Take(1).First().Key;
    }

    void FireProjectile()
    {
        Debug.Log($"pew pew");
        Projectile projectileInstance = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectileInstance.transform.parent = transform;
        projectileInstance.GetComponent<Projectile>().Shoot(this, _target, 20);// HERE: projectile power
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

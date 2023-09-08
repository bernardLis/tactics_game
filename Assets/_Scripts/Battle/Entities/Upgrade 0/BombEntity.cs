using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombEntity : BattleCreatureRanged
{
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] GameObject _explosionEffect;

    List<GameObject> _hitInstances = new();
    GameObject _explosionEffectInstance;

    public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true, bool hasGrave = true)
    {
        yield return ManageCreatureAbility();
        Invoke(nameof(CleanUp), 2f);
        yield return base.Die(attacker, hasLoot, hasGrave);
    }

    protected override IEnumerator CreatureAbility()
    {
        yield return base.CreatureAbility();

        _explosionEffectInstance = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        _explosionEffectInstance.transform.parent = _GFX.transform;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == Team) continue; // splash damage is player friendly
                if (entity.IsDead) continue;

                StartCoroutine(entity.GetHit(Creature, 50));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(Creature.HitPrefab, collider.bounds.center, q);
                hitInstance.transform.parent = entity.transform;
                _hitInstances.Add(hitInstance);
            }
        }
    }

    void CleanUp()
    {
        if (_explosionEffectInstance != null)
            Destroy(_explosionEffectInstance);

        for (int i = _hitInstances.Count - 1; i >= 0; i--)
        {
            Destroy(_hitInstances[i]);
            _hitInstances.RemoveAt(i);
        }
    }

}

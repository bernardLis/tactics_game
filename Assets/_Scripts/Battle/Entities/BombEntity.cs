using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BombEntity : BattleEntityRanged
{
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] GameObject _explosionEffect;

    List<GameObject> _hitInstances = new();
    GameObject _explosionEffectInstance;

    public override IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        // explode
        _explosionEffectInstance = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        _explosionEffectInstance.transform.parent = _GFX.transform;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<BattleEntity>(out BattleEntity entity))
            {
                if (entity == this) continue;
                StartCoroutine(entity.GetHit(this));
                Quaternion q = Quaternion.Euler(0, -90, 0); // face default camera position
                GameObject hitInstance = Instantiate(ArmyEntity.HitPrefab, _opponent.Collider.bounds.center, q);
                _hitInstances.Add(hitInstance);
            }
        }


        Invoke("CleanUp", 2f);

        Debug.Log($"special die after everything");
        StartCoroutine(base.Die(attacker, ability));
        yield return null;

        // yield return base.Die(attacker, ability);
    }

    void CleanUp()
    {
        Debug.Log($"cleanup");
        if (_explosionEffectInstance != null)
            Destroy(_explosionEffectInstance);

        for (int i = _hitInstances.Count - 1; i >= 0; i--)
        {
            Destroy(_hitInstances[i]);
            _hitInstances.RemoveAt(i);
        }
    }

}

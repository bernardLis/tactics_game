using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleForestTree : MonoBehaviour
{
    Ability _ability;
    List<BattleEntity> _entitiesInCollider = new();

    [SerializeField] GameObject[] _treeGFX;
    [SerializeField] GameObject _effect;

    public void Initialize(Ability ability)
    {
        _ability = ability;
    }

    public void Fire(Vector3 pos)
    {
        pos.y = 0;
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        _treeGFX[Random.Range(0, _treeGFX.Length)].SetActive(true);
        gameObject.SetActive(true);
        DamageOnInception();
        _effect.SetActive(true);
        transform.DOScale(1, 0.5f)
                 .SetDelay(0.2f)
                 .OnComplete(() => StartCoroutine(FireCoroutine()));
    }

    void DamageOnInception()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider c in colliders)
        {
            if (c.TryGetComponent(out BattleBreakableVase bbv))
            {
                bbv.TriggerBreak();
                continue;
            }
            if (c.TryGetComponent(out BattleEntity entity))
            {
                if (entity.Team == 0) continue; // TODO: hardcoded team number
                StartCoroutine(entity.GetHit(_ability));
            }
        }
    }

    IEnumerator FireCoroutine()
    {
        _entitiesInCollider.Clear();

        float endTime = Time.time + _ability.GetDuration();
        while (Time.time < endTime)
        {
            if (_entitiesInCollider.Count > 0)
            {
                BattleEntity entity = _entitiesInCollider[Random.Range(0, _entitiesInCollider.Count)];
                // rotate to face entity
                transform.DOLookAt(entity.transform.position, 0.2f);
                yield return new WaitForSeconds(0.2f);
                // punch rotation to 90 degrees forward
                Vector3 originalRot = transform.eulerAngles;
                Vector3 rot = transform.eulerAngles;
                rot.z = 75;
                transform.DORotate(rot, 0.1f)
                         .OnComplete(() => transform.DORotate(originalRot, 0.1f));
                StartCoroutine(entity.GetHit(_ability));
            }
            yield return new WaitForSeconds(0.7f);
        }
        transform.DOScale(0, 0.5f)
                 .OnComplete(Disable);

    }

    void Disable()
    {
        foreach (GameObject g in _treeGFX)
            g.SetActive(false);
        _effect.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return; // TODO: hardcoded team number
            battleEntity.OnDeath += RemoveEntityFromList;
            _entitiesInCollider.Add(battleEntity);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return; // TODO: hardcoded team number
            RemoveEntityFromList(battleEntity, null);
        }
    }

    void RemoveEntityFromList(BattleEntity entity, EntityFight ignored)
    {
        if (_entitiesInCollider.Contains(entity))
            _entitiesInCollider.Remove(entity);
    }
}

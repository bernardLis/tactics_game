using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BattleEarthSpike : MonoBehaviour
{
    [SerializeField] GameObject _GFX;
    [SerializeField] GameObject _col;
    [SerializeField] ParticleSystem _spikes;
    Ability _ability;

    List<BattleEntity> _entitiesInCollider = new();


    public void Initialize(Ability ability)
    {
        _ability = ability;
        _ability.OnLevelUp += OnAbilityLevelUp;

        transform.localScale = Vector3.one * _ability.GetScale();
    }

    void OnAbilityLevelUp()
    {
        transform.localScale = Vector3.one * _ability.GetScale();
    }

    public void Fire(Vector3 pos, Vector3 rot)
    {
        pos.y = 0;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);
        gameObject.SetActive(true);

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        _GFX.SetActive(true);
        _col.SetActive(true);

        ParticleSystem.MainModule main = _spikes.main;
        main.startLifetime = _ability.GetDuration();

        yield return DamageCoroutine();
        yield return new WaitForSeconds(0.5f);

        _col.SetActive(false);
        _GFX.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator DamageCoroutine()
    {
        float endTime = Time.time + _ability.GetDuration();
        while (Time.time < endTime)
        {
            List<BattleEntity> currentEntities = new(_entitiesInCollider);
            foreach (BattleEntity entity in currentEntities)
                StartCoroutine(entity.GetHit(_ability));
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
            bbv.TriggerBreak();

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

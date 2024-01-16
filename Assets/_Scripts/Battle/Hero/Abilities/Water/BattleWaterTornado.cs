using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleWaterTornado : MonoBehaviour
{
    // collision *0.5 scale
    Ability _ability;

    [SerializeField] ParticleSystem[] _psDurationChange;

    float _angularSpeed = 1f;
    float _circleRad = 3f;
    float _currentAngle;

    BattleHero _hero;


    List<BattleEntity> _entitiesInCollider = new();


    public void Initialize(Ability ability)
    {
        _ability = ability;
        _hero = BattleManager.Instance.BattleHero;
    }

    public void Fire(Vector3 pos)
    {
        pos.y = 0;
        transform.position = pos;
        gameObject.SetActive(true);
        _angularSpeed = Random.Range(0.1f, 0.4f);
        _circleRad = Vector3.Distance(pos, _hero.transform.position);

        foreach (ParticleSystem ps in _psDurationChange)
        {
            var main = ps.main;
            main.startLifetime = _ability.GetDuration();
        }

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        transform.localScale = Vector3.one * _ability.GetScale();
        StartCoroutine(DealDamage());

        // I would like tornado to follow a circular path
        Vector3 fixedPos = _hero.transform.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        float endTime = Time.time + _ability.GetDuration();
        while (Time.time < endTime)
        {
            _currentAngle += _angularSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Sin(_currentAngle), 0, Mathf.Cos(_currentAngle)) * _circleRad;
            transform.position = fixedPos + offset;

            yield return new WaitForFixedUpdate();
        }

        transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    IEnumerator DealDamage()
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
            _entitiesInCollider.Add(battleEntity);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return; // TODO: hardcoded team number
            if (_entitiesInCollider.Contains(battleEntity))
                _entitiesInCollider.Remove(battleEntity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleMeteors : MonoBehaviour
{
    [SerializeField] GameObject _circle;    // start lifetime determines how long the circle will be growing (4 seconds now)
    [SerializeField] GameObject _meteor;

    Ability _ability;
    List<BattleEntity> _entitiesInCollider = new();

    public void Initialize(Ability ability)
    {
        _ability = ability;
    }


    public void Fire(Vector3 pos)
    {
        pos.y = 0;
        transform.position = pos;

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        transform.localScale = Vector3.one * _ability.GetScale();

        _circle.transform.localScale = Vector3.one;
        _meteor.transform.localScale = Vector3.one;

        ManageCircles();
        yield return new WaitForSeconds(2f);
        ManageMeteors();
        StartCoroutine(DealDamage());
        yield return new WaitForSeconds(_ability.GetDuration());
        yield return _circle.transform.DOScale(0, 1f).WaitForCompletion();
        _meteor.transform.DOScale(0, 0.5f);
    }

    void ManageCircles()
    {
        _circle.SetActive(true);

        foreach (Transform child in _circle.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            ps.Simulate(0.0f, true, true);
            ps.Play();
        }
    }

    void ManageMeteors()
    {
        ParticleSystem ps = _meteor.GetComponent<ParticleSystem>();
        ps.Simulate(0.0f, true, true);

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.radius = _ability.GetScale();
        int burstCount = Mathf.FloorToInt(_ability.GetDuration());
        short burstCountShort = (short)burstCount;
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, burstCountShort, burstCountShort, 20, 0.1f)
                });

        _meteor.SetActive(true);

        ps.Play();
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

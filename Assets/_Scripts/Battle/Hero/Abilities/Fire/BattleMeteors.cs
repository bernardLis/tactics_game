using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleMeteors : MonoBehaviour
{
    [SerializeField] GameObject _circle;    // start lifetime determines how long the circle will be growing (4 seconds now)
    [SerializeField] GameObject _meteor;

    Ability _ability;

    public bool IsActive;

    public void Initialize(Ability ability)
    {
        _ability = ability;
    }


    public void Fire(Vector3 pos)
    {
        IsActive = true;
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
        float endTime = Time.time + _ability.GetDuration() - 0.5f;
        while (Time.time < endTime)
        {
            ExplosionDamage();
            yield return new WaitForSeconds(0.5f);
        }

        yield return _circle.transform.DOScale(0, 1f).WaitForCompletion();
        _meteor.transform.DOScale(0, 0.5f).OnComplete(Deactivate);
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

    void ExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_meteor.transform.position, _ability.GetScale());
        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent(out BattleBreakableVase bbv))
            {
                bbv.TriggerBreak();
                continue;
            }
            if (hit.TryGetComponent(out BattleEntity be))
            {
                if (be.Team == 0) continue; // TODO: hardcoded team number
                StartCoroutine(be.GetHit(_ability));
            }
        }
    }

    void Deactivate()
    {
        IsActive = false;
    }
}

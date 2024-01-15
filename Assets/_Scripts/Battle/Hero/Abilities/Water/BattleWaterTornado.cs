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
        StartCoroutine(DamageCoroutine());

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

    IEnumerator DamageCoroutine()
    {
        float endTime = Time.time + _ability.GetDuration();
        while (Time.time < endTime)
        {
            Damage();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Damage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
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
}

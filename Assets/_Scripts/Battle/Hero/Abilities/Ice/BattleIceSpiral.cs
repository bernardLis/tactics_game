using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIceSpiral : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] Collider _col;

    [SerializeField] ParticleSystem _iceSpikes;
    [SerializeField] ParticleSystem _snow;
    [SerializeField] ParticleSystem[] _delayedEffects; // -1f of duration
    [SerializeField] ParticleSystem _iceExplosion; // -0.2f of duration

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
        _gfx.SetActive(true);
        _col.gameObject.SetActive(true);

        SetDurations();
        StartCoroutine(DealDamage());

        yield return new WaitForSeconds(_ability.GetDuration() + 0.5f);
        _col.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f); // for ice to disappear
        _gfx.SetActive(false);
        gameObject.SetActive(false);
    }

    void SetDurations()
    {
        var iceSpikesMain = _iceSpikes.main;
        iceSpikesMain.startLifetime = _ability.GetDuration();

        var snowMain = _snow.main;
        snowMain.startLifetime = _ability.GetDuration();

        foreach (ParticleSystem ps in _delayedEffects)
        {
            var psMain = ps.main;
            psMain.startDelay = _ability.GetDuration() - 1f;
        }

        var iceExplosionMain = _iceExplosion.main;
        iceExplosionMain.startDelay = _ability.GetDuration() - 0.1f;
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

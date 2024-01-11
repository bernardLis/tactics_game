using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BattleEarthSpike : MonoBehaviour
{
    [SerializeField] GameObject _GFX;
    [SerializeField] GameObject _col;
    Ability _ability;

    public bool IsActive;

    float _endTime;

    public void Initialize(Ability ability, BattleAbility battleAbility)
    {
        _ability = ability;
        _ability.OnLevelUp += OnAbilityLevelUp;

        transform.localScale = Vector3.one * _ability.GetScale();
    }

    public void Fire(Vector3 pos, Vector3 rot)
    {
        IsActive = true;
        pos.y = 0;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        _GFX.SetActive(true);
        _endTime = Time.time + _ability.GetDuration();

        while (Time.time < _endTime)
        {
            _col.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _col.SetActive(false);
        }

        _col.SetActive(false);
        _GFX.SetActive(false);
        IsActive = false;
    }

    void OnAbilityLevelUp()
    {
        transform.localScale = Vector3.one * _ability.GetScale();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
            bbv.TriggerBreak();

        if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return; // TODO: hardcoded team number
            StartCoroutine(battleEntity.GetHit(_ability));
        }
    }
}

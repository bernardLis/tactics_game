using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BattleEarthSpike : BattleAbilityObjectDmgOverTime
{
    [SerializeField] GameObject _GFX;
    [SerializeField] GameObject _col;
    [SerializeField] ParticleSystem _spikes;

    public override void Initialize(Ability ability)
    {
        base.Initialize(ability);
        transform.localScale = Vector3.one * _ability.GetScale();
    }

    protected override void OnAbilityLevelUp()
    {
        transform.localScale = Vector3.one * _ability.GetScale();
    }

    public override void Execute(Vector3 pos, Quaternion rot)
    {
        pos.y = 0;
        base.Execute(pos, Quaternion.identity);
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        _GFX.SetActive(true);
        _col.SetActive(true);

        ParticleSystem.MainModule main = _spikes.main;
        main.startLifetime = _ability.GetDuration();

        yield return DamageCoroutine(Time.time + _ability.GetDuration());
        yield return new WaitForSeconds(0.5f);

        _col.SetActive(false);
        _GFX.SetActive(false);
        gameObject.SetActive(false);
    }


}

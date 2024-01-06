using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BattleEarthSlash : MonoBehaviour
{
    [SerializeField] GameObject _effect;
    [SerializeField] GameObject _col;
    Ability _ability;
    BattleAbility _battleAbility;

    public void Initialize(Ability ability, BattleAbility battleAbility)
    {
        _ability = ability;
        _ability.OnLevelUp += OnAbilityLevelUp;
        battleAbility.OnAbilityFire += Fire;

        transform.localScale = Vector3.one * _ability.GetScale();
    }

    void Fire(Vector3 pos, Vector3 rot)
    {
        // transform.position = pos;
        // transform.rotation = Quaternion.Euler(rot);
        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        _effect.SetActive(true);
        _col.SetActive(true);

        Vector3 colliderRotation = new(90f, 0f, -45f);
        _col.transform.DOLocalRotate(colliderRotation, 0.3f)
                    .OnComplete(() =>
                    {
                        _col.SetActive(false);
                    });

        yield return new WaitForSeconds(1f);
        _effect.SetActive(false);
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

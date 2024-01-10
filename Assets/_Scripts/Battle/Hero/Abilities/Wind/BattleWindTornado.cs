using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleWindTornado : MonoBehaviour
{

    [SerializeField] float _speed = 5f;
    Ability _ability;

    public void Initialize(Ability ability)
    {
        _ability = ability;
    }
    
    public void Fire(Vector3 pos, Quaternion q)
    {
        pos.y = 0;
        transform.SetPositionAndRotation(pos, q);
        gameObject.SetActive(true);

        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        transform.localScale = Vector3.one * _ability.GetScale();

        float elapsedTime = 0;
        while (elapsedTime < _ability.GetDuration())
        {
            transform.position += _speed * Time.fixedDeltaTime * transform.forward;
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.DOScale(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
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

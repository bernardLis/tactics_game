using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTornado : MonoBehaviour
{

    [SerializeField] float _speed = 5f;
    Ability _ability;

    public void Initialize(Ability ability)
    {
        _ability = ability;

        StartCoroutine(GoForward(3f)); // TODO: hardcoded duration
        Invoke(nameof(DestroySelf), 3.2f);
    }

    IEnumerator GoForward(float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position += transform.forward * Time.deltaTime * _speed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
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

    void DestroySelf()
    {
        Destroy(gameObject);
    }

}

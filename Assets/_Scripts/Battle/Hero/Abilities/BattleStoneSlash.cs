using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Codice.CM.Client.Differences.Graphic;
public class BattleStoneSlash : MonoBehaviour
{
    [SerializeField] GameObject col;
    Ability _ability;

    public void Initialize(Ability ability)
    {
        _ability = ability;

        Invoke(nameof(DestroySelf), 2f);

        Vector3 colliderRotation = new(90f, 0f, -45f);
        col.transform.DOLocalRotate(colliderRotation, 0.3f)
                    .OnComplete(() => col.SetActive(false));
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision " + collision.gameObject.name);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElementalSphere : MonoBehaviour
{
    BattleManager _battleManager;
    [SerializeField] Element _element;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += SuckInCorpse;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = _element.Color;
    }

    void SuckInCorpse(BattleEntity be)
    {
        // maybe the corpses are added to a list and the sphere checks the distance every now and then
        // and sucks in the ones that are close enough
        if (be is not BattleMinion) return;
        if (be.Entity.Element != _element) return;
        // I need to come up wiht an idea for how to handle this
        // they need to grab the corpses as they go only the ones that are close to them

        be.transform.DOMove(transform.position, 1f).OnComplete(() =>
        {
            be.gameObject.SetActive(false);
            // HERE: disable the cleanup of the corpse in battlemanager
        });

    }
}

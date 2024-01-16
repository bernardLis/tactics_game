using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BattleAbilityForest : BattleAbility
{
    [SerializeField] GameObject _effect;

    float _radius = 12f;

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0, 0f, 0f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();
        _effect.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            Vector3 pos = _battleAreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position,
                                            _radius * _ability.GetScale());
            BattleForestTree tree = GetInactiveAbilityObject() as BattleForestTree;
            tree.Execute(pos, Quaternion.identity);
        }
        yield return new WaitForSeconds(3f);
        _effect.SetActive(false);
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, _radius, 1f);
    }
#endif
}

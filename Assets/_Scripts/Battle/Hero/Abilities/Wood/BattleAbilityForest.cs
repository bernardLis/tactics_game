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
    [SerializeField] GameObject _treePrefab;
    List<BattleForestTree> _treePool = new();

    float _radius = 12f;

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0, 0f, 0f);

        for (int i = 0; i < _ability.GetAmount(); i++)
            InitializeTree();
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();
        _effect.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            Vector3 pos = _battleAreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position,
                                            _radius * _ability.GetScale());
            BattleForestTree tree = GetInactiveTree();
            tree.Fire(pos);
        }
        yield return new WaitForSeconds(3f);
        _effect.SetActive(false);
    }

    BattleForestTree GetInactiveTree()
    {
        foreach (BattleForestTree p in _treePool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeTree();
    }


    BattleForestTree InitializeTree()
    {
        GameObject instance = Instantiate(_treePrefab, Vector3.zero,
                                Quaternion.identity, _battleManager.AbilityHolder);
        BattleForestTree tree = instance.GetComponent<BattleForestTree>();
        tree.Initialize(_ability);
        _treePool.Add(tree);
        return tree;
    }




#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, _radius, 1f);

    }
#endif


}

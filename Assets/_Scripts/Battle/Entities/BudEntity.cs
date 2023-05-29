using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BudEntity : BattleEntityRanged
{
    [SerializeField] GameObject _effect;
    GameObject _effectInstance;
    protected override void Start()
    {
        _hasSpecialMove = true;
        base.Start();
    }

    protected override IEnumerator SpecialAbility()
    {
        _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);

        Vector3 point = ClosesPositionWithClearLOS();
        transform.position = point;

        Invoke("CleanUp", 2f);

        return base.SpecialAbility();
    }

    void CleanUp()
    {
        if (_effectInstance != null)
            Destroy(_effectInstance);
    }

}

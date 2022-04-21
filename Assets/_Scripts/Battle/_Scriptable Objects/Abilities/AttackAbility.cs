using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Attack Ability")]
public class AttackAbility : Ability
{
    AttackTriggerable _attackTriggerable;

    public bool IsRetaliation { get; private set; }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _attackTriggerable = obj.GetComponent<AttackTriggerable>();
    }

    public async override Task AbilityLogic(GameObject target)
    {
        // check if target is valid // TODO: DO I need that? Maybe triggerable should take care of it.
        var attackableObject = target.GetComponent<IAttackable<GameObject, Ability>>();
        if (attackableObject == null)
            return;

        // interact
        await _attackTriggerable.Attack(target, this, IsRetaliation);

        SetIsRetaliation(false);
    }

    public void SetIsRetaliation(bool isRet) { IsRetaliation = isRet; }
}

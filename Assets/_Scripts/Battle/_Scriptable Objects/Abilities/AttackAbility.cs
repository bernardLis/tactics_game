using UnityEngine;
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

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        await base.TriggerAbility(target);

        // check if target is valid
        var attackableObject = target.GetComponent<IAttackable<GameObject, Ability>>();
        if (attackableObject == null)
            return false;

        // interact
        if (!await _attackTriggerable.Attack(target, this, IsRetaliation))
            return false;

        SetIsRetaliation(false);

        return true;
    }

    public void SetIsRetaliation(bool isRet) { IsRetaliation = isRet; }
}

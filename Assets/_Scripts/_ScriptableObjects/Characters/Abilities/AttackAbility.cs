using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Attack Ability")]
public class AttackAbility : Ability
{
    protected AttackTriggerable _attackTriggerable;

    public bool IsRetaliation { get; private set; }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _attackTriggerable = obj.GetComponent<AttackTriggerable>();
    }

    public override bool CanBeUsed() { return ManaCheck() && TargetCheck(); }

    public override bool IsTargetViable(GameObject target)
    {
        // you can attack friends but I am not going to return them as viable targets
        if (target.TryGetComponent(out IAttackable<GameObject, Ability> attackable)
            && !target.CompareTag(CharacterGameObject.tag))
            return true;
        return false;
    }

    public async override Task AbilityLogic(Vector3 pos)
    {
        // interact
        await _attackTriggerable.Attack(pos, this, IsRetaliation);
        SetIsRetaliation(false);
    }

    public void SetIsRetaliation(bool isRet) { IsRetaliation = isRet; }
}

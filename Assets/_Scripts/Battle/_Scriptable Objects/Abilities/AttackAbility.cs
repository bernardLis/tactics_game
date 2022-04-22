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

    public async override Task AbilityLogic(Vector3 pos)
    {
        // interact
        await _attackTriggerable.Attack(pos, this, IsRetaliation);

        SetIsRetaliation(false);
    }

    public void SetIsRetaliation(bool isRet) { IsRetaliation = isRet; }
}

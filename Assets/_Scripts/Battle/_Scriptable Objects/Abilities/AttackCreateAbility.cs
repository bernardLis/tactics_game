using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/AttackCreate Ability")]
public class AttackCreateAbility : AttackAbility
{
    [Header("Create Ability")]
    public GameObject CreatedObject;

    public async override Task AbilityLogic(Vector3 pos)
    {
        GameObject hit = await _attackTriggerable.Attack(pos, this, IsRetaliation);
        SetIsRetaliation(false);

        // if projectile, spawn the object where projectile hits
        Vector3 tileEffectPosition = pos;
        if (hit != null)
            tileEffectPosition = hit.transform.position;

        GameObject obj = Instantiate(CreatedObject, tileEffectPosition, Quaternion.identity);
        await obj.GetComponent<ICreatable<Vector3, Ability>>().Initialize(tileEffectPosition, this);
    }


}

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
        await base.AbilityLogic(pos);
        GameObject obj = Instantiate(CreatedObject, pos, Quaternion.identity);
        await obj.GetComponent<ICreatable<Vector3, Ability>>().Initialize(pos, this);
    }


}

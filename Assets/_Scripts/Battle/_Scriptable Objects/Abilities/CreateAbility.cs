using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Create Ability")]
public class CreateAbility : Ability
{
    // this ability summons a game object on the tile:
    // boulder
    // physical damage shield
    // ranged attacks shield
    // these objects can have their own logic but that's not important to the ability
    [Header("Create Ability")]
    public GameObject CreatedObject;

    public async override Task AbilityLogic(Vector3 pos)
    {
        GameObject obj = Instantiate(CreatedObject, pos, Quaternion.identity);
        await obj.GetComponent<ICreatable<Vector3, Ability>>().Initialize(pos, this);
    }
}

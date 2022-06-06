using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Create Ability")]
public class CreateAbility : Ability
{

    [Header("Create Ability")]
    public GameObject CreatedObject;

    public async override Task AbilityLogic(Vector3 pos)
    {
        _battleUI.DisplayBattleLog($"{CharacterGameObject.name} creates {CreatedObject.name}.");
        _stats.Character.GetExp(10);

        GameObject obj = Instantiate(CreatedObject, pos, Quaternion.identity);
        await obj.GetComponent<ICreatable<Vector3, Ability, string>>().Initialize(pos, this, CharacterGameObject.tag);
    }
}




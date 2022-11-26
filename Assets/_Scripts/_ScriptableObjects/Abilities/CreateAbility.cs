using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "ScriptableObject/Ability/Create Ability")]
public class CreateAbility : Ability
{

    [Header("Create Ability")]
    public GameObject CreatedObject;

    public async override Task AbilityLogic(Vector3 pos)
    {
        _battleUI.DisplayBattleLog(new BattleLogLine(new Label($"{CharacterGameObject.name} creates {CreatedObject.name}."), BattleLogLineType.Ability));

        GameObject obj = Instantiate(CreatedObject, pos, Quaternion.identity);
        await obj.GetComponent<ICreatable<Vector3, Ability, string>>().Initialize(pos, this, CharacterGameObject.tag);
    }
}




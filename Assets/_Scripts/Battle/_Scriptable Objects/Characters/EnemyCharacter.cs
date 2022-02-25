using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Enemy")]
public class EnemyCharacter : Character
{
    public Brain EnemyBrain;

    public override void Create(Dictionary<string, object> item, List<Ability> abilities)
    {
        base.Create(item, abilities);
        EnemyBrain = (Brain)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/Battle/_Scriptable Objects/Brains/{item["EnemyBrain"]}.asset", typeof(Brain));
    }
    
    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        var clone = Instantiate(EnemyBrain);
        clone.Initialize(obj);
    }
}

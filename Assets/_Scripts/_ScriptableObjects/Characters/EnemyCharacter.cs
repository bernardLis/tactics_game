using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Enemy")]
public class EnemyCharacter : Character
{
    public Brain EnemyBrain;

    public virtual void CreateEnemy(int level, Brain brain)
    {
        CharacterName = brain.Name;
        Portrait = brain.Portrait;
        Level = level;

        Power = Mathf.FloorToInt(level * brain.PowerMultiplier) + 5;
        MaxHealth = 100 + Mathf.FloorToInt(level * 10);
        MaxMana = 50 + Mathf.FloorToInt(level * 5);
        Armor = 0;
        MovementRange = 4;


        Body = brain.Body;
        Weapon = brain.Weapon;
        EnemyBrain = brain;
    }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        var clone = Instantiate(EnemyBrain);
        clone.Initialize(obj, this);
    }

    protected override void BaseExpGain(int gain)
    {
        // Enemies don't get exp
        return;
    }
}

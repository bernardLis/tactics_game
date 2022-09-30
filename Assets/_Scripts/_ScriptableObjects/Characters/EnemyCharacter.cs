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
        Body = brain.Body;
        Weapon = brain.Weapon;
        EnemyBrain = brain;

        if (brain.ManualStats)
        {
            Level = brain.Level;
            Power = brain.Power;
            MaxHealth = brain.MaxHealth;
            MaxMana = brain.MaxMana;
            Armor = brain.Armor;
            MovementRange = brain.MovementRange;
            return;
        }

        Level = level;
        Power = Mathf.FloorToInt(level * brain.PowerMultiplier) + 5;
        MaxHealth = 100 + Mathf.FloorToInt(level * 2);
        MaxMana = 50 + Mathf.FloorToInt(level);
        Armor = 0; // TODO:
        MovementRange = 4;
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

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

        Strength = Mathf.FloorToInt(level * brain.StrengthMultiplier) + 5;
        Intelligence = Mathf.FloorToInt(level * brain.IntelligenceMultiplier) + 5;
        Agility = Mathf.FloorToInt(level * brain.AgilityMultiplier) + 5;
        Stamina = Mathf.FloorToInt(level * brain.StaminaMultiplier) + 5;

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
}

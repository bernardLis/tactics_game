using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Enemy")]
public class EnemyCharacter : Character
{
    public Brain EnemyBrain;

    public void CreateEnemy(int level, Brain brain)
    {
        CharacterName = "Jason";
        Portrait = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Character/Portrait/Eyck.jpg", typeof(Sprite));
        Level = level;
        // TODO: samehitng different - like take brain into consideration and stuff
        Strength = level * 5;
        Intelligence = level * 5;
        Agility = level * 5;
        Stamina = level * 5;
        Body = brain.Body;
        Weapon = brain.Weapon;
        EnemyBrain = brain;
    }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        var clone = Instantiate(EnemyBrain);
        clone.Initialize(obj);
    }
}

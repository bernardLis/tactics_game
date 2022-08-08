using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global/Global Character Upgrade")]
public class GlobalCharacterUpgrade : GlobalUpgrade
{
    public StatType affectedStat;
    public int Value;

    public override void Initialize(Character character)
    {
        character.ChangeStat(affectedStat.ToString(), Value);
    }
}

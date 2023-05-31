using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Entity Ability")]
public class EntityAbility : BaseScriptableObject
{
    public string Description;

    public Sprite Icon;
    public int Cooldown;


}

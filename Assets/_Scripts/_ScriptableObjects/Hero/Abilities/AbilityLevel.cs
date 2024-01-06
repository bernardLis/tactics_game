using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability Level")]
public class AbilityLevel : BaseScriptableObject
{
    public int Level;

    public float Power;
    public float Cooldown;
    public float Scale;
    public int Amount;


}

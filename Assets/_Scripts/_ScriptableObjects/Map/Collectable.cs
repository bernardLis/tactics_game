using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : BaseScriptableObject
{
    public int Amount;
    public Sprite Sprite;

    public virtual void Initialize() { }
    public virtual void Collect(MapHero hero)
    {
        Debug.Log($"{hero.Character.CharacterName} collects {this.name}");
    }
}

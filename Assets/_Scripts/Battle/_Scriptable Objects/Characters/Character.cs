using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Player")]
public class Character : BaseScriptableObject
{
    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string CharacterName = "Default";
    public Sprite Portrait;

    [Header("Stats")]
    public int Strength; // how strong you hit
    public int Intelligence; // maxMana depends on intelligence (also how strong the spell dmg is)
    public int Agility; // influences range
    public int Stamina; // influences maxHealth

    [Header("Equipment")]
    public Equipment Shield;
    public Equipment Body;
    public Equipment Feet;
    public Equipment Hair;
    public Equipment Hands;
    public Equipment Helmet;
    public Equipment Legs;
    public Equipment Torso;
    public Weapon Weapon;

    [Header("Abilities")]
    [Tooltip("For now just defend, basic attack is from the weapon")]
    public Ability[] BasicAbilities; 
    public Ability[] CharacterAbilities;

    public virtual void Initialize(GameObject obj)
    {
        // TODO: this could be done in a loop
        // you have to learn reflections

        // get all
        Transform bodyObj = obj.transform.Find("Body");
        if (Body != null)
            Body.Initialize(bodyObj.gameObject);

        // TODO: deal with object none;
        Transform feetObj = bodyObj.transform.Find("Feet");
        if (Feet != null)
            Feet.Initialize(feetObj.gameObject);

        Transform hairObj = bodyObj.transform.Find("Hair");
        if (Hair != null)
            Hair.Initialize(hairObj.gameObject);

        Transform handsObj = bodyObj.transform.Find("Hands");
        if (Hands != null)
            Hands.Initialize(handsObj.gameObject);

        Transform helmetObj = bodyObj.transform.Find("Helmet");
        if (Helmet != null)
            Helmet.Initialize(helmetObj.gameObject);

        Transform legsObj = bodyObj.transform.Find("Legs");
        if (Legs != null)
            Legs.Initialize(legsObj.gameObject);

        Transform torsoObj = bodyObj.transform.Find("Torso");
        if (Torso != null)
            Torso.Initialize(torsoObj.gameObject);

        Transform shieldObj = bodyObj.transform.Find("Shield");
        if (Shield != null)
            Shield.Initialize(shieldObj.gameObject);

        Transform weaponObj = bodyObj.transform.Find("Weapon");
        if (Weapon != null)
            Weapon.Initialize(weaponObj.gameObject);
    }
}

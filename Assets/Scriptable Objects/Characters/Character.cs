using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Player")]
public class Character : ScriptableObject
{
    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string characterName = "Default";
    public Sprite portrait;

    [Header("Stats")]
    public int maxHealth;
    public int armor;
    public int strength;
    public int intelligence;
    public int maxMana;
    public int movementRange;

    [Header("Equipment")]
    public Equipment body;
    public Equipment feet;
    public Equipment hair;
    public Equipment hands;
    public Equipment helmet;
    public Equipment legs;
    public Equipment torso;
    public Equipment shield;
    public Equipment weapon;

    [Header("Abilities")]
    public Ability[] characterAbilities;

    public virtual void Initialize(GameObject obj)
    {
        // TODO: this could be done in a loop
        // you have to learn reflections

        // TODO: dunno if I should be instantiating the object coz now I am using the same object always

        // get all
        Transform bodyObj = obj.transform.Find("Body");
        if (body != null)
            body.Initialize(bodyObj.gameObject);

        // TODO: deal with object none;
        Transform feetObj = bodyObj.transform.Find("Feet");
        if (feet != null)
            feet.Initialize(feetObj.gameObject);

        Transform hairObj = bodyObj.transform.Find("Hair");
        if (hair != null)
            hair.Initialize(hairObj.gameObject);

        Transform handsObj = bodyObj.transform.Find("Hands");
        if (hands != null)
            hands.Initialize(handsObj.gameObject);

        Transform helmetObj = bodyObj.transform.Find("Helmet");
        if (helmet != null)
            helmet.Initialize(helmetObj.gameObject);

        Transform legsObj = bodyObj.transform.Find("Legs");
        if (legs != null)
            legs.Initialize(legsObj.gameObject);

        Transform torsoObj = bodyObj.transform.Find("Torso");
        if (torso != null)
            torso.Initialize(torsoObj.gameObject);

        Transform shieldObj = bodyObj.transform.Find("Shield");
        if (shield != null)
            shield.Initialize(shieldObj.gameObject);

        Transform weaponObj = bodyObj.transform.Find("Weapon");
        if (weapon != null)
            weapon.Initialize(weaponObj.gameObject);

    }
}

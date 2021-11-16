using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(menuName = "Character/Enemy")]
public class EnemyCharacter : Character
{
    public EnemyAI enemyBrain;

    public EquipmentDatabase equipmentDatabase;

    public override void Initialize(GameObject obj)
    {
        // TODO: understand reflections and get it done in a nice loop
        // enemy needs a body
        if (body == null)
            body = RandomizeEq(EquipmentSlot.BODY);

        // TODO: need to differentiate between a piece of eq left blank on purpose and not
        // TODO: it would be fun if eq had influence on enemy stats
        // so you start fighting with enemies in rugs and end fighting enemies in golden armors
        // but that's for another day;
        if (feet == null && Random.value > 0.5)
            feet = RandomizeEq(EquipmentSlot.FEET);
        if (hair == null && Random.value > 0.5)
            hair = RandomizeEq(EquipmentSlot.HAIR);
        if (hands == null && Random.value > 0.5)
            hands = RandomizeEq(EquipmentSlot.HANDS);
        if (helmet == null && Random.value > 0.5)
            helmet = RandomizeEq(EquipmentSlot.HELMET);
        if (legs == null && Random.value > 0.5)
            legs = RandomizeEq(EquipmentSlot.LEGS);
        if (torso == null && Random.value > 0.5)
            torso = RandomizeEq(EquipmentSlot.TORSO);
        if (shield == null && Random.value > 0.5)
            shield = RandomizeEq(EquipmentSlot.SHIELD);
        if (weapon == null && Random.value > 0.5)
            weapon = (Weapon) RandomizeEq(EquipmentSlot.WEAPON);

        base.Initialize(obj);
        enemyBrain = obj.AddComponent(typeof(EnemyAI)) as EnemyAI;
    }

    Equipment RandomizeEq(EquipmentSlot slot)
    {
        // TODO: this could be made smarter, right?
        if (slot == EquipmentSlot.BODY)
            return equipmentDatabase.allBodies[Random.Range(0, equipmentDatabase.allBodies.Count)];
        if (slot == EquipmentSlot.FEET)
            return equipmentDatabase.allFeet[Random.Range(0, equipmentDatabase.allFeet.Count)];
        if (slot == EquipmentSlot.HAIR)
            return equipmentDatabase.allHair[Random.Range(0, equipmentDatabase.allHair.Count)];
        if (slot == EquipmentSlot.HANDS)
            return equipmentDatabase.allHands[Random.Range(0, equipmentDatabase.allHands.Count)];
        if (slot == EquipmentSlot.HELMET)
            return equipmentDatabase.allHelmets[Random.Range(0, equipmentDatabase.allHelmets.Count)];
        if (slot == EquipmentSlot.LEGS)
            return equipmentDatabase.allLegs[Random.Range(0, equipmentDatabase.allLegs.Count)];
        if (slot == EquipmentSlot.TORSO)
            return equipmentDatabase.allTorsos[Random.Range(0, equipmentDatabase.allTorsos.Count)];
        if (slot == EquipmentSlot.SHIELD)
            return equipmentDatabase.allShields[Random.Range(0, equipmentDatabase.allShields.Count)];
        if (slot == EquipmentSlot.WEAPON)
            return equipmentDatabase.allWeapons[Random.Range(0, equipmentDatabase.allWeapons.Count)];

        return null;
    }


}

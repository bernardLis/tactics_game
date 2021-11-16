using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/EquipmentDatabase")]
public class EquipmentDatabase : ScriptableObject
{
    public List<Equipment> allBodies = new();
    public List<Equipment> allFeet = new();
    public List<Equipment> allHair = new();
    public List<Equipment> allHands = new();
    public List<Equipment> allHelmets = new();
    public List<Equipment> allLegs = new();
    public List<Equipment> allTorsos = new();
    public List<Equipment> allShields = new();
    public List<Weapon> allWeapons = new();
}

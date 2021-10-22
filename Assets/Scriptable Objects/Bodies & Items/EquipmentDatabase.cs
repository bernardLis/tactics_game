using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/EquipmentDatabase")]
public class EquipmentDatabase : ScriptableObject
{
    public List<Equipment> allBodies = new List<Equipment>();
    public List<Equipment> allFeet = new List<Equipment>();
    public List<Equipment> allHair = new List<Equipment>();
    public List<Equipment> allHands = new List<Equipment>();
    public List<Equipment> allHelmets = new List<Equipment>();
    public List<Equipment> allLegs = new List<Equipment>();
    public List<Equipment> allTorsos = new List<Equipment>();
    public List<Equipment> allShields = new List<Equipment>();
    public List<Equipment> allWeapons = new List<Equipment>();
}

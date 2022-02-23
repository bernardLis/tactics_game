using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Equipment/EquipmentDatabase")]
public class EquipmentDatabase : ScriptableObject
{
    public List<Equipment> AllBodies = new();
    public List<Equipment> AllFeet = new();
    public List<Equipment> AllHair = new();
    public List<Equipment> AllHands = new();
    public List<Equipment> AllHelmets = new();
    public List<Equipment> AllLegs = new();
    public List<Equipment> AllTorsos = new();
    public List<Equipment> AllShields = new();
    public List<Weapon> AllWeapons = new();
}

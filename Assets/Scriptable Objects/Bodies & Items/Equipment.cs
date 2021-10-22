using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot { BODY, FEET, HAIR, HANDS, HELMET, LEGS, TORSO, SHIELD, WEAPON }
public enum Gender { MALE, FEMALE }


[CreateAssetMenu(menuName = "Equipment/NewEquipment")]
public class Equipment : ScriptableObject
{
	public string eName;
	public Sprite icon;
	public RuntimeAnimatorController animatorController;
	public EquipmentSlot eSlot;
	public Gender gender;

	// TODO: prolly a slot 
	// TODO: stats? 
	// TODO: maybe it should be a parent class, and helmet, torso etc. should be dervied from this class
	public virtual void Initialize(GameObject equipmentHandler)
	{
		// equip = set animator controller of a correct game object
		equipmentHandler.GetComponent<SpriteRenderer>().sprite = icon;
		equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = animatorController;
	}

}


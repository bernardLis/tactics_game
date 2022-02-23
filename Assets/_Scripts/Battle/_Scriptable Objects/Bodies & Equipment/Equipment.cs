using UnityEngine;

public enum EquipmentSlot { Body, Feet, Hair, Hands, Helmet, Legs, Torso, Shield, Weapon }
public enum Gender { Male, Female }


[CreateAssetMenu(menuName = "ScriptableObject/Equipment/NewEquipment")]
public class Equipment : BaseScriptableObject
{
	public Sprite Icon;
	public RuntimeAnimatorController AnimatorController;
	public EquipmentSlot Slot;
	public Gender Gender;

	// TODO: prolly a slot 
	// TODO: stats? 
	// TODO: maybe it should be a parent class, and helmet, torso etc. should be dervied from this class
	public virtual void Initialize(GameObject equipmentHandler)
	{
		// equip = set animator controller of a correct game object
		equipmentHandler.GetComponent<SpriteRenderer>().sprite = Icon;
		equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;
	}

}


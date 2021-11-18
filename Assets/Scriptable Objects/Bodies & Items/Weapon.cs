using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType { SLASH, THRUST, SHOOT, ANY }

[CreateAssetMenu(menuName = "Weapon")]
public class Weapon : Equipment
{
	public WeaponType weaponType;
	public Ability basicAttack;

	// https://answers.unity.com/questions/158319/overriding-an-initial-value-in-a-subclass.html
	public void Reset()
	{
		eSlot = EquipmentSlot.WEAPON;
	}

	public override void Initialize(GameObject equipmentHandler)
	{
		//base.Initialize(equipmentHandler);
		equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = animatorController;
		//equipmentHandler.GetComponent<WeaponHolder>().SetWeapon(this);

		// if it is a bow, activate gameobject of the 
		if (weaponType == WeaponType.SHOOT)
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(true);
		else
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(false);
	}

}

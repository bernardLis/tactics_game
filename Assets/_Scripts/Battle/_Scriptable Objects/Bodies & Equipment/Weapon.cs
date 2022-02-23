using UnityEngine;
public enum WeaponType { Slash, Thrust, Shoot, Any }

[CreateAssetMenu(menuName = "ScriptableObject/Weapon")]
public class Weapon : Equipment
{
	public WeaponType WeaponType;
	public Ability BasicAttack;

	// https://answers.unity.com/questions/158319/overriding-an-initial-value-in-a-subclass.html
	public void Reset()
	{
		Slot = EquipmentSlot.Weapon;
	}

	public override void Initialize(GameObject equipmentHandler)
	{
		//base.Initialize(equipmentHandler);
		equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;
		//equipmentHandler.GetComponent<WeaponHolder>().SetWeapon(this);

		// if it is a bow, activate gameobject of the 
		if (WeaponType == WeaponType.Shoot)
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(true);
		else
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(false);
	}

}

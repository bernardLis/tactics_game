using UnityEngine;
public enum WeaponType { Slash, Thrust, Shoot, Any }

[CreateAssetMenu(menuName = "ScriptableObject/Weapon")]
public class Weapon : Equipment
{
	public WeaponType WeaponType;
	public Ability BasicAttack;

	public override void Initialize(GameObject equipmentHandler)
	{
		//base.Initialize(equipmentHandler);
		equipmentHandler.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;
		//equipmentHandler.GetComponent<WeaponHolder>().SetWeapon(this);

		// if it is a bow, activate arrow gameobject
		if (WeaponType == WeaponType.Shoot)
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(true);
		else
			equipmentHandler.transform.Find("Arrow").gameObject.SetActive(false);
	}

}

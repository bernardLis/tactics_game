using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
	public Weapon Weapon { get; private set; }

	public void SetWeapon(Weapon weapon)
	{
		Weapon = weapon;
	}
}

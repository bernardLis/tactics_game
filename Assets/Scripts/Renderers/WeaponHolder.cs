using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
	public Weapon weapon { get; private set; }

	public void SetWeapon(Weapon _weapon)
	{
		weapon = _weapon;
	}
}

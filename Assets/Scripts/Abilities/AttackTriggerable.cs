using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerable : MonoBehaviour
{
	CharacterStats myStats;
	CharacterStats targetStats;
	CharacterInteractionController characterInteractionController;

	void Awake()
	{
		characterInteractionController = GetComponent<CharacterInteractionController>();
		myStats = GetComponent<CharacterStats>();
	}

	public void Attack(GameObject target)
	{
		int damage = characterInteractionController.selectedAbility.value + myStats.strength.GetValue();
		targetStats = target.GetComponent<CharacterStats>();

		// face the target character
		Vector2 dir = target.transform.position - transform.position;
		characterInteractionController.Face(dir);

		if (targetStats != null)
		{
			// TODO: dunno if this is a correct place to do that...
			AudioSource audioSource;
			audioSource = AudioScript.instance.transform.GetComponent<AudioSource>();
			audioSource.clip = characterInteractionController.selectedAbility.aSound;
			audioSource.Play();

			targetStats.TakeDamage(damage);
			characterInteractionController.FinishCharacterTurn();
		}
		else
		{
			Debug.Log("target stats are null");
		}
	}
}

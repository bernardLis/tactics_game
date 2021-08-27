using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTriggerable : MonoBehaviour
{
	CharacterStats myStats;
	CharacterStats targetStats;
	CharacterInteractionController characterInteractionController;

	Pushable pushable;

	void Awake()
	{
		characterInteractionController = GetComponent<CharacterInteractionController>();
		myStats = GetComponent<CharacterStats>();
	}

	public void Push(GameObject target)
	{
		// face the target character
		Vector2 dir = target.transform.position - transform.position;
		characterInteractionController.Face(dir);

		// player can push characters/stones
		// TODO: pushing characters with lerp breaks the A*
		Vector3 pushDir = (target.transform.position - transform.position).normalized;

		pushable = target.GetComponent<Pushable>();
		if (pushable != null)
		{
			// TODO: dunno if this is a correct place to do that...
			AudioSource audioSource;
			audioSource = AudioScript.instance.transform.GetComponent<AudioSource>();
			audioSource.clip = characterInteractionController.selectedAbility.aSound;
			audioSource.Play();

			pushable.IsPushed(pushDir);
			characterInteractionController.FinishCharacterTurn();
		}
	}
}

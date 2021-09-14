using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FMPlayerMovementController : MonoBehaviour
{


	[SerializeField]
	float movementSpeed = 4f;
	FMIsometricCharacterRenderer isoRenderer;

	Rigidbody2D rbody;
	Vector2 inputVector;

	RushUI rushUI;
	[SerializeField]
	GameObject rushEffect;
	bool rushButtonIsHeld = false;
	float rushChargingSpeed = 4f;
	//float rushLimit = 4.5f;
	float rushStartTime;
	float rushForcedReleaseTime;
	float rushSecondsToForcedRelease = 1.2f;
	float percentToLimit = 0f;

	Vector2 rushStrength;
	bool rushForceReleaseTriggered;

	public InputMaster controls;

	public bool isSneaking { get; private set; }

	void Awake()
	{
		rushUI = GameUI.instance.GetComponent<RushUI>();

		controls = new InputMaster();

		controls.FMPlayer.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
		// to reset on key release
		controls.FMPlayer.Movement.canceled += ctx => inputVector = Vector2.zero;

		controls.FMPlayer.Sneak.performed += ctx => Sneak();
		// to reset on key release
		controls.FMPlayer.Sneak.canceled += ctx => SneakReset();

		controls.FMPlayer.Rush.performed += ctx => RushPressed();
		controls.FMPlayer.Rush.canceled += ctx => RushReleased();


		rbody = GetComponent<Rigidbody2D>();
		isoRenderer = GetComponentInChildren<FMIsometricCharacterRenderer>();
	}

	void OnEnable()
	{
		controls.FMPlayer.Enable();
	}

	void OnDisable()
	{
		controls.FMPlayer.Disable();
	}

	void Move(Vector2 direction)
	{
		inputVector = direction;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// rush is charged
		if (rushButtonIsHeld)
		{
			rushStrength += inputVector * rushChargingSpeed * Time.fixedDeltaTime;

			// TODO: there has to be a better way to get the %
			rushStartTime += Time.fixedDeltaTime;
			percentToLimit = 1 - ((rushForcedReleaseTime - rushStartTime) / rushSecondsToForcedRelease);

			// you have x seconds to charge the rush, before I forcefully release it;
			if (Time.fixedTime < rushForcedReleaseTime)
			{
				rushUI.DrawRushUI(transform.position, rushStrength, percentToLimit);
			}
			/*
			// After rush reaches 4 tiles it will be forcefully released within 0.5s
			float limitCheck = Mathf.Abs(rushStrength.x) + Mathf.Abs(rushStrength.y);
			float percentToLimit = limitCheck / rushLimit;
			if (time < rushLimit)
			{
				rushUI.DrawRushUI(transform.position, rushStrength, percentToLimit);
			}
			*/
			else
			{
				//rushUI.DrawRushUI(transform.position, rushStrength, percentToLimit);
				// force release the rush once it goes over the limit and is not released;
				if (!rushForceReleaseTriggered)
				{
					rushForceReleaseTriggered = true;
					ForceRushRelease();
				}
			}
			return;
		}
		// normal movmenent code;
		Vector2 currentPos = rbody.position;

		inputVector = Vector2.ClampMagnitude(inputVector, 1);

		Vector2 movement = inputVector * movementSpeed;
		Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

		isoRenderer.SetDirection(movement);
		rbody.MovePosition(newPos);
		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
	}

	void Sneak()
	{
		movementSpeed = 1f;
		isSneaking = true;
	}

	void SneakReset()
	{
		movementSpeed = 3f;
		isSneaking = false;
	}

	void RushPressed()
	{
		rushButtonIsHeld = true;
		rushStartTime = Time.fixedTime;
		rushForcedReleaseTime = Time.fixedTime + rushSecondsToForcedRelease;
	}

	void RushReleased()
	{
		Vector3 originalPosition = transform.position;

		rushButtonIsHeld = false;

		Vector2 currentPos = rbody.position;
		Vector2 newPos = currentPos + rushStrength;
		Vector2 dir = rushStrength.normalized;

		// TODO: check for obstacles on the way if there is an obcstalce you should stop before it
		// Cast a ray
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(currentPos, newPos));
		// If it hits something... 
		// 3 is unpassable layer
		if (hit.collider != null && hit.collider.gameObject.layer == 3)
		{
			// move to the tile before the obstacle
			newPos = new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y) - dir;
		}

		rbody.MovePosition(newPos);
		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

		// don't spawn effect if there is no rush
		if (rushStrength != Vector2.zero)
		{
			float angle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;
			// TODO: destory after the animation is finished 
			Destroy(Instantiate(rushEffect, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)) as GameObject, 1f);
			Destroy(Instantiate(rushEffect, originalPosition, Quaternion.AngleAxis(angle, Vector3.forward)) as GameObject, 1f);
		}

		rushUI.HideRushUI();
		rushStrength = Vector2.zero;
	}

	void ForceRushRelease()
	{
		rushForceReleaseTriggered = false;
		//rushStrength = Vector2.zero;
		RushReleased();
	}
}

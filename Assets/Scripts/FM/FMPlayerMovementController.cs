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

	[SerializeField]
	GameObject rushEffect;
	bool rushButtonIsHeld = false;
	Vector2 rushStrength;

	public InputMaster controls;

	public bool isSneaking { get; private set; }

	void Awake()
	{
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
		controls.Enable();
	}

	void OnDisable()
	{
		controls.Disable();
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
			rushStrength += inputVector * movementSpeed * Time.fixedDeltaTime;
			GameUI.instance.DrawRushUI(transform.position, rushStrength);
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
		Debug.Log("sneak");
		movementSpeed = 1f;
		isSneaking = true;
	}

	void SneakReset()
	{
		Debug.Log("SneakReset");
		movementSpeed = 3f;
		isSneaking = false;
	}

	void RushPressed()
	{
		print("Rush(");
		rushButtonIsHeld = true;
		// get direction and force

	}
	void RushReleased()
	{
		print("RushReset()");
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
			Destroy(Instantiate(rushEffect, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)) as GameObject, 1f);

		}

		GameUI.instance.HideRushUI();
		rushStrength = Vector2.zero;

		// TODO: change to destory after particle system lifetime;

	}
}

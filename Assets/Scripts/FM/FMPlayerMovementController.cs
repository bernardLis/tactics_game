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
}

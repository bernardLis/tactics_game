﻿using UnityEngine;
using System.Collections;

public class BasicCameraFollow : MonoBehaviour
{

	Vector3 startingPosition;
	public Transform followTarget;
	Vector3 targetPos;
	public float moveSpeed;

	#region Singleton
	public static BasicCameraFollow instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Camera Follow found");
			return;
		}
		instance = this;
	}
	#endregion

	void Start()
	{
		startingPosition = transform.position;
	}

	void Update()
	{
		if (followTarget != null)
		{
			targetPos = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
			Vector3 velocity = (targetPos - transform.position) * moveSpeed;
			transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
		}
	}
}


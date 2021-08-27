using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public InputMaster controls;

	float zoomSpeed = 1f;
	float minZoom = 4f;
	float maxZoom = 15f;

	Camera cam;

	void Awake()
	{
		cam = GetComponent<Camera>();

		// controls
		// https://www.youtube.com/watch?v=Pzd8NhcRzVo
		// registering input callbacks (keyboard)
		controls = new InputMaster();
		controls.Camera.ZoomIn.performed += ctx => ZoomIn();
		controls.Camera.ZoomOut.performed += ctx => ZoomOut();
	}
	void OnEnable()
	{
		controls.Enable();
	}
	void OnDisable()
	{
		controls.Disable();
	}

	void ZoomIn()
	{
		if (cam.orthographicSize > minZoom)
		{
			cam.orthographicSize -= zoomSpeed;
		}
	}
	void ZoomOut()
	{
		if (cam.orthographicSize < maxZoom)
		{
			cam.orthographicSize += zoomSpeed;
		}
	}
}

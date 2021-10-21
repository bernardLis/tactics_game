using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    PlayerInput playerInput;

    float zoomSpeed = 1f;
    float minZoom = 4f;
    float maxZoom = 15f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void OnEnable()
    {
        // TODO: does this cost a lot, can I do something smarter
        // maybe characters don't need interaction controllers, they just need to know what abilities they have and I will 
        playerInput = MovePointController.instance.GetComponent<PlayerInput>();

        // inputs
        playerInput.actions["ZoomIn"].performed += ctx => ZoomIn();
        playerInput.actions["ZoomOut"].performed += ctx => ZoomOut();
    }
    void OnDisable()
    {
        if(playerInput == null)
            return;

        playerInput.actions["ZoomIn"].performed -= ctx => ZoomIn();
        playerInput.actions["ZoomOut"].performed -= ctx => ZoomOut();
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

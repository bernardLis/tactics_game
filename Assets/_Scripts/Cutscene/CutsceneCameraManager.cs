using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutsceneCameraManager : CameraManager
{
    protected override void Awake()
    {
        base.Awake();
        _cam = GetComponent<Camera>();
    }

    protected override void Start()
    {

    }

    public void PanCamera(Vector3 direction, int duration)
    {
        Vector3 newPos = transform.position + direction;
        transform.DOMove(newPos, duration);
    }

    public void ZoomCameraIn(int duration)
    {
        float orthoSize = _cam.orthographicSize;
        _cam.DOOrthoSize(orthoSize - 1, duration);
    }

    public void ZoomCameraOut(int duration)
    {
        float orthoSize = _cam.orthographicSize;
        _cam.DOOrthoSize(orthoSize + 1, duration);
    }


}

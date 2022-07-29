using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutsceneCameraManager : BattleCameraManager
{
    protected override void Awake()
    {
        base.Awake();
        _cam = GetComponent<Camera>();
    }

    protected override void Start()
    {

    }

    public void PanCamera(Vector3 direction, float duration)
    {
        Vector3 newPos = transform.position + direction;
        transform.DOMove(newPos, duration);
    }

    public void ZoomCameraIn(float duration)
    {
        float orthoSize = _cam.orthographicSize;
        _cam.DOOrthoSize(orthoSize - 1, duration);
    }

    public void ZoomCameraOut(float duration)
    {
        float orthoSize = _cam.orthographicSize;
        _cam.DOOrthoSize(orthoSize + 1, duration);
    }


}

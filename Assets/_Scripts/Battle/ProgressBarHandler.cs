using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarHandler : MonoBehaviour
{
    Camera _cam;

    MeshRenderer _meshRenderer;
    Material _mat;

    public void Initialize()
    {
        // _cam = Camera.main;

        _meshRenderer = GetComponent<MeshRenderer>();
        _mat = _meshRenderer.material;
    }

    // void LateUpdate()
    // {
    //     if (!isActiveAndEnabled) return;
    //     if (_cam == null) _cam = Camera.main;
    //     // transform.LookAt(transform.position + _cam.transform.forward);
    //     Quaternion rot = Quaternion.LookRotation(transform.position - _cam.transform.position);

    //     // float y = Mathf.Clamp(rot.eulerAngles.y, 140, 220);
    //     transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 270);
    // }

    public void SetProgress(float progress)
    {
        _mat.SetFloat("_fill", progress);
    }

    public void SetFillColor(Color fillColor)
    {
        _mat.SetColor("_fillColor", fillColor);
    }

    public void SetBorderColor(Color borderColor)
    {
        _mat.SetColor("_borderColor", borderColor);
    }

    public void ShowProgressBar()
    {
        _meshRenderer.enabled = true;
    }

    public void HideProgressBar()
    {
        _meshRenderer.enabled = false;
    }

}

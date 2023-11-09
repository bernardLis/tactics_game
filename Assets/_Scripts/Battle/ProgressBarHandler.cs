using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarHandler : MonoBehaviour
{
    MeshRenderer _meshRenderer;
    Material _mat;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _mat = _meshRenderer.material;
    }

    public void SetProgress(float progress)
    {
        _mat.SetFloat("_Fill", progress);
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

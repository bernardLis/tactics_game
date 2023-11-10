using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarHandler : MonoBehaviour
{
    MeshRenderer _meshRenderer;
    Material _mat;

    public void Initialize()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _mat = _meshRenderer.material;
    }

    public void SetProgress(float progress)
    {
        _mat.SetFloat("_Fill", progress);
    }

    public void SetFillColor(Color fillColor)
    {
        _mat.SetColor("_Fill_Color", fillColor);
    }

    public void SetColors(Color borderColor, Color highlightColor)
    {
        _mat.SetColor("_Shell_Color", borderColor);
        _mat.SetColor("_Fill_Highlight_Color", highlightColor);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectShaders : MonoBehaviour
{
    GameManager _gameManager;

    Shader _litShader;

    Shader _dissolveShader;
    Shader _grayScaleShader;
    Shader _sepiaToneShader;



    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void LitShader()
    {
        if (_litShader == null)
            _litShader = GameManager.Instance.GameDatabase.LitShader;

        List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Texture2D tex = mat.mainTexture as Texture2D;
            mat.shader = _litShader;
            mat.SetTexture("_Base_Texture", tex);
        }

    }

    public void GrayScale()
    {
        if (_grayScaleShader == null)
            _grayScaleShader = GameManager.Instance.GameDatabase.GrayScaleShader;

        List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Texture2D tex = mat.mainTexture as Texture2D;
            mat.shader = _grayScaleShader;
            mat.SetTexture("_Base_Texture", tex);
        }
    }

    public void Dissolve(float time, bool isReverse)
    {
        if (_dissolveShader == null)
            _dissolveShader = GameManager.Instance.GameDatabase.DissolveShader;

        List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Texture2D tex = mat.mainTexture as Texture2D;
            mat.shader = _dissolveShader;
            mat.SetTexture("_Base_Texture", tex);

            float startValue = isReverse ? 1f : 0f;
            float endValue = isReverse ? 0f : 1f;

            mat.SetFloat("_Dissolve_Value", startValue);
            DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), startValue, endValue, time);
        }
    }
}
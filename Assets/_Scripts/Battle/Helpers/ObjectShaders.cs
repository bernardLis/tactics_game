using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectShaders : MonoBehaviour
{
    GameManager _gameManager;

    Shader _litShader;
    Shader _particlesUnlitShader;


    Shader _dissolveShader;
    Shader _grayScaleShader;
    Shader _sepiaToneShader;

    List<Shader> _originalShaders = new();


    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void LitShader()
    {
        if (_litShader == null)
            _litShader = GameManager.Instance.GameDatabase.LitShader;
        if (_particlesUnlitShader == null)
            _particlesUnlitShader = GameManager.Instance.GameDatabase.ParticlesUnlitShader;

        List<Renderer> renderers = new(GetComponentsInChildren<Renderer>());
        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Texture2D tex = mat.mainTexture as Texture2D;
            mat.shader = _litShader;
            mat.SetTexture("_Base_Texture", tex);

            if (r.GetComponent<ParticleSystem>() != null)
                mat.shader = _particlesUnlitShader;

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
        _originalShaders = new();
        foreach (Renderer r in renderers)
        {
            Material mat = r.material;
            _originalShaders.Add(mat.shader);

            Vector2 texScale = mat.mainTextureScale; // tiling
            Texture2D tex = mat.mainTexture as Texture2D;
            Texture2D metallicMap = null;
            if (mat.HasProperty("_MetallicGlossMap"))
                metallicMap = mat.GetTexture("_MetallicGlossMap") as Texture2D;

            mat.shader = _dissolveShader;
            mat.SetTexture("_Base_Texture", tex);
            if (metallicMap != null)
                mat.SetTexture("_R_Metallic_G_Occulsion_A_Smoothness", metallicMap);
            mat.SetVector("_Tiling", texScale);

            float startValue = isReverse ? 1f : 0f;
            float endValue = isReverse ? 0f : 1f;

            mat.SetFloat("_Dissolve", startValue);
            DOTween.To(x => mat.SetFloat("_Dissolve", x), startValue, endValue, time);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleEntityShaders : MonoBehaviour
{
    GameManager _gameManager;
    Shader _dissolveShader;
    Shader _originalShader;

    public event Action OnDissolveComplete;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _dissolveShader = _gameManager.GameDatabase.DissolveShader;
    }

    public void Dissolve(float time, bool isReverse)
    {
        if (_dissolveShader == null)
            _dissolveShader = GameManager.Instance.GameDatabase.DissolveShader;

        Material mat = GetComponentInChildren<Renderer>().material;
        Texture2D tex = mat.mainTexture as Texture2D;
        _originalShader = mat.shader;
        mat.shader = _dissolveShader;
        mat.SetTexture("_Base_Texture", tex);

        float startValue = isReverse ? 1f : 0f;
        float endValue = isReverse ? 0f : 1f;

        mat.SetFloat("_Dissolve_Value", startValue);
        DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), startValue, endValue, time)
                .OnComplete(() =>
                {
                    // mat.shader = originalShader;
                    OnDissolveComplete?.Invoke();
                });
    }
}
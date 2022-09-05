using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLineController : MonoBehaviour
{
    //https://www.youtube.com/watch?v=VvjIUIlso9M
    LineRenderer _lineRenderer;
    [SerializeField] Texture[] _textures;
    int _animationStep;
    float _fps = 30f;
    float _fpsCoutner;
    int currentPosition = 0;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.material.SetTexture("_MainTex", _textures[0]);
    }

    public void Electrify(Vector3 startPos)
    {
        _lineRenderer.SetPosition(0, startPos);
    }

    public void AddPosition(Vector3 pos)
    {
        if (_lineRenderer == null)
            return;
        currentPosition++;
        _lineRenderer.positionCount = currentPosition + 1;
        _lineRenderer.SetPosition(currentPosition, pos);
    }

    void Update()
    {
        if (_lineRenderer == null)
            return;

        _fpsCoutner += Time.deltaTime;
        if (_fpsCoutner >= 1f / _fps)
        {
            _animationStep++;
            if (_animationStep == _textures.Length)
                _animationStep = 0;
            _lineRenderer.material.SetTexture("_MainTex", _textures[_animationStep]);
            _fpsCoutner = 0f;
        }
    }
}

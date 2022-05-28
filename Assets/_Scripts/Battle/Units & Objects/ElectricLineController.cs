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

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Electrify(Vector3 startPos, Vector3 endPos)
    {
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    void Update()
    {
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

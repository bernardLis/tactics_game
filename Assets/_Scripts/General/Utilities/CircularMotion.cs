using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public float StartAngle = 0;
    public float RotationSpeed = 0.1f;
    public float CircleRadius = 800;
    public float ElevationOffset = 0;

    Vector3 _positionOffset;

    [SerializeField] float _angle;

    void Start()
    {
        _angle = StartAngle;
    }

    private void LateUpdate()
    {
        _positionOffset.Set(
            Mathf.Cos(_angle) * CircleRadius,
            ElevationOffset,
            Mathf.Sin(_angle) * CircleRadius
        );
        transform.position = Vector3.zero + _positionOffset;
        _angle += Time.deltaTime * RotationSpeed;
    }
}

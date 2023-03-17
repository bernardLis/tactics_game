using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    Camera _cam;

    float dampTime = 0.15f;
    Vector3 velocity = Vector3.zero;
    Transform _target;

    void Start() { _cam = GetComponent<Camera>(); }

    void Update()
    {
        if (_target == null)
            return;
        Vector3 point = _cam.WorldToViewportPoint(_target.position);
        Vector3 delta = _target.position - _cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    }

    public void SetTarget(Transform t) { _target = t; }
}

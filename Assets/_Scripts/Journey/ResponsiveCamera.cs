using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

//https://www.youtube.com/watch?v=gFWQHordrtA
public class ResponsiveCamera : MonoBehaviour
{
    float _buffer = 2f;
    Camera _cam;

    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    // TODO: make it smoother       
    public void SetOrthoSize()
    {
        var bounds = new Bounds();

        foreach (var col in FindObjectsOfType<Collider2D>())
            bounds.Encapsulate(col.bounds);

        bounds.Expand(_buffer);
        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;

        float size = Mathf.Max(horizontal, vertical) * 0.5f;
        Vector3 center = bounds.center + new Vector3(0, 0, -10);

        transform.DOMove(center, 1f);
        DOTween.To(() => _cam.orthographicSize, x => _cam.orthographicSize = x, size, 1f)
            .SetEase(Ease.InSine);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

//https://www.youtube.com/watch?v=gFWQHordrtA
public class ResponsiveCamera : MonoBehaviour
{
    float buffer = 2f;
    Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // TODO: make it smoother       
    public void SetOrthoSize()
    {
        var bounds = new Bounds();

        foreach (var col in FindObjectsOfType<Collider2D>())
            bounds.Encapsulate(col.bounds);

        bounds.Expand(buffer);
        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        float size = Mathf.Max(horizontal, vertical) * 0.5f;
        Vector3 center = bounds.center + new Vector3(0, 0, -10);

        //cam.transform.position = center;
        transform.DOMove(center,1f);
        DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, size, 1f)
            .SetEase(Ease.InSine);
    }

}

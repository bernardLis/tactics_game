using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Material _mat;
    // Start is called before the first frame update
    void Start()
    {
        _mat = GetComponentInChildren<SpriteRenderer>().material;
        _mat.SetColor("_OutlineColor", Color.black);
        _mat.SetFloat("_OutlineThickness", 0);
    }

    public void OnPointerEnter(PointerEventData evt) { Highlight(); }

    public void OnPointerExit(PointerEventData evt) { ClearHighlight(); }

    public void Highlight()
    {
        _mat.SetFloat("_OutlineThickness", 10);
        _mat.SetColor("_OutlineColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
    }

    public void ClearHighlight()
    {
        _mat.SetFloat("_OutlineThickness", 0);
        _mat.SetColor("_OutlineColor", Color.black);
    }
}

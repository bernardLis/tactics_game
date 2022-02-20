using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KnowledgeOptionBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public KnowledgeEventOption knowledgeEventOption;
    public Image image;
    public void Initialize(KnowledgeEventOption _option)
    {
        knowledgeEventOption = _option;

        image.sprite = _option.sprite;
        image.rectTransform.sizeDelta = _option.miniatureSpriteSize;
        image.GetComponentInParent<Canvas>().worldCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<BoxCollider2D>().enabled = false;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        pos.z = 0f;
        gameObject.transform.position = pos;

    }


}

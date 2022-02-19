using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class KnowledgeOptionBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer Down");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On begin drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("On end drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position = eventData.pointerCurrentRaycast.worldPosition;
    }


}

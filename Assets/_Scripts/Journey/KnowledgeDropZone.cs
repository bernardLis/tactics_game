using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

// TODO: better name?
public class KnowledgeDropZone : MonoBehaviour, IDropHandler
{

    public KnowledgeEventOption knowledgeEventOption;

    public void Initialize(KnowledgeEventOption _option)
    {
        knowledgeEventOption = _option;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("on drop: " + gameObject.name);
        if (eventData.pointerDrag == null)
            return;

        KnowledgeOptionBehaviour droppedObject = eventData.pointerDrag.GetComponent<KnowledgeOptionBehaviour>();
        SpriteRenderer droppedObjectRenderer = droppedObject.GetComponentInChildren<SpriteRenderer>();
        KnowledgeEventOption droppedOption = droppedObject.knowledgeEventOption;

        if (droppedOption == knowledgeEventOption)
            droppedObjectRenderer.color = Color.green;
        else
            droppedObjectRenderer.color = Color.red;

    }

}

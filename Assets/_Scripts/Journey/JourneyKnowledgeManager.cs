using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class JourneyKnowledgeManager : MonoBehaviour
{
    [Header("Unity Setup")]
    public GameObject knowledgeOptionPrefab;
    public GameObject dropZonePrefab;

    // UI
    UIDocument UIDocument;

    public KnowledgeEvent[] events;
    KnowledgeEvent selectedEvent;
    void Start()
    {
        UIDocument = GetComponent<UIDocument>();

        selectedEvent = events[0]; // TODO: if many events select a random one, make sure they don't reapear in the same save
        CreateOptions();
        CreateDropZones();
    }

    void CreateOptions()
    {
        int x = -7;
        int y = 4;
        foreach (KnowledgeEventOption option in selectedEvent.options)
        {
            GameObject optionObject = Instantiate(knowledgeOptionPrefab, new Vector3(x, y), Quaternion.identity);
            optionObject.GetComponent<KnowledgeOptionBehaviour>().Initialize(option);
            y -= 2;
        }

    }

    void CreateDropZones()
    {
        int x = -2;
        int y = 0;
        foreach (KnowledgeEventOption option in selectedEvent.options)
        {
            Label txt = new Label(option.text);
            UIDocument.rootVisualElement.Add(txt);
            Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(txt.panel, new Vector3(x - 1, y + 2), Camera.main);
            Debug.Log("poanel pos: " + panelPos);
            txt.AddToClassList("label");
            txt.style.left = panelPos.x;
            txt.style.top = panelPos.y;

            GameObject optionDropZone = Instantiate(dropZonePrefab, new Vector3(x, y), Quaternion.identity);
            optionDropZone.GetComponent<KnowledgeDropZone>().Initialize(option);
            x += 3;
        }

    }

}

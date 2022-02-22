using UnityEngine;


public enum JourneyNodeType { Start, End, Battle, Knowledge, Chest, Blacksmith, Fire, Boss, Event }
[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite icon;
    public JourneyNodeType nodeType;
    public string sceneToLoad;

    [HideInInspector] public GameObject gameObject;
    SpriteRenderer sr;
    [HideInInspector] public JourneyNodeBehaviour journeyNodeBehaviour;

    public void Initialize(GameObject _self)
    {
        gameObject = _self;
        gameObject.name = name;
        gameObject.transform.localScale = new Vector3(3f, 3f);

        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        journeyNodeBehaviour = gameObject.GetComponent<JourneyNodeBehaviour>();
        sr.sprite = icon;
    }

    // TODO: do I want to do it from scriptable Object?
    public void Select()
    {
        sr.color = Color.black;
    }

}

[System.Serializable]
public struct JourneyNodeData
{
    public int pathIndex;
    public int nodeIndex;
}



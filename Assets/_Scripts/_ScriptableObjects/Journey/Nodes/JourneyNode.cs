using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Journey/Node")]
public class JourneyNode : BaseScriptableObject
{
    public Sprite Icon;
    public JourneyNodeType NodeType;
    public string SceneToLoad;

    public Reward Reward;

    [HideInInspector] public GameObject GameObject;
    SpriteRenderer _spriteRenderer;
    //[HideInInspector] public JourneyNodeBehaviour JourneyNodeBehaviour;
    public bool WasVisited;

    [HideInInspector] public int PathIndex;
    [HideInInspector] public int NodeIndex;

    public virtual void Initialize(GameObject self)
    {
        GameObject = self;
        GameObject.name = name;
        GameObject.transform.localScale = new Vector3(3f, 3f);

        _spriteRenderer = GameObject.GetComponentInChildren<SpriteRenderer>();
       // JourneyNodeBehaviour = GameObject.GetComponent<JourneyNodeBehaviour>();
        _spriteRenderer.sprite = Icon;
    }

    // TODO: do I want to do it from scriptable Object?
    public void Select()
    {
        _spriteRenderer.color = Color.black;
        WasVisited = true;
    }

    public JourneyNodeData Serialize()
    {
        return new JourneyNodeData(PathIndex, NodeIndex);

    }
}

[System.Serializable]
public struct JourneyNodeData
{
    public int PathIndex;
    public int NodeIndex;

    public JourneyNodeData(int pathIndex, int nodeIndex)
    {
        PathIndex = pathIndex;
        NodeIndex = nodeIndex;
    }
}



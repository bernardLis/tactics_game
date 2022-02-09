using UnityEngine;

public class JourneyNodeBehaviour : MonoBehaviour
{
    public JourneyNode journeyNode;

    public void Initialize(JourneyNode _jn)
    {
        journeyNode = _jn;
        gameObject.name = _jn.name;
        GetComponentInChildren<SpriteRenderer>().sprite = _jn.icon;
    }

}

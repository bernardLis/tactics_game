using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNodeScript : MonoBehaviour
{

    AbilityNode _abilityNode;

    [SerializeField] float offsetXStart = 0.5f;
    [SerializeField] float offsetXEnd = -0.5f;


    public void InitializeScript(AbilityNode abilityNode)
    {
        _abilityNode = abilityNode;

        GetComponent<SpriteRenderer>().sprite = abilityNode.Icon;
    }

    public void PaintLine(AbilityNodeScript targetScript)
    {
        ShapesPainter p = GetComponent<ShapesPainter>();
        Vector3 start = new(transform.position.x + offsetXStart, transform.position.y, transform.position.z);
        Vector3 end = new(targetScript.transform.position.x + offsetXEnd, targetScript.transform.position.y, targetScript.transform.position.z);
        Debug.Log($"start {start}");
        Debug.Log($"end {end}");
        p.DrawLine(start, end);
    }
}

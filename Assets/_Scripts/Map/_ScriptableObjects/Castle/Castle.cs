using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Castle")]
public class Castle : BaseScriptableObject
{
    public Vector2 MapPosition;
    public Sprite Sprite;

    public virtual void Initialize() { }


}

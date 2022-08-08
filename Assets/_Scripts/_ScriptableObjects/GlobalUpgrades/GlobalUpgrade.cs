using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpgrade : BaseScriptableObject
{
    public int Price;
    public Sprite Sprite;
    public string Tooltip;

    public virtual void Initialize(Character character)
    {
        // meant to be overwritten;
    }


}

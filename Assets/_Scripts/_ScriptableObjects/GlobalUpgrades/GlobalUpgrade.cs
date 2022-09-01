using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalUpgrade : BaseScriptableObject
{
    public int Price;
    public Sprite Sprite;
    public string Tooltip;
    public UpgradeType UpgradeType;

    public virtual void Initialize()
    {

    }
    public virtual void Initialize(Character character)
    {

    }

}

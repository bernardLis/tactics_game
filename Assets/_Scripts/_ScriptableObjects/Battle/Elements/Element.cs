using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Element")]
public class Element : BaseScriptableObject
{
    public ElementName ElementName;
    public Sprite Icon;
    public ColorVariable Color;
    public string Description;
    public Element StrongAgainst;
    public Element WeakAgainst;

    public EffectHolder VFXEffect;
}

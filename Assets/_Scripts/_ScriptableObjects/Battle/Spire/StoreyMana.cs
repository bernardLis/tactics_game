using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Mana")]
public class StoreyMana : Storey
{
    public List<StoreyUpgrade> Blabla = new();
    public int CurrentMaxTroopsLevel;
    
    public List<Creature> bkasd = new();
}

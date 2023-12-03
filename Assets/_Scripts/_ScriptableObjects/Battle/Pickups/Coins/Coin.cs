using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Coin")]
public class Coin : Pickup
{
    public Vector2Int AmountRange;

    public int Amount;

    public override void Initialize()
    {
        Amount = Random.Range(AmountRange.x, AmountRange.y);
    }

    public override void Collected(Hero hero)
    {
        GameManager.Instance.ChangeGoldValue(Amount);
    }

}

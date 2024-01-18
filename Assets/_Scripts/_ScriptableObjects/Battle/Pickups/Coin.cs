
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Hero/Coin")]
    public class Coin : Pickup
    {
        public Vector2Int AmountRange;

        public int Amount;

        public override void Initialize()
        {
            Amount = Random.Range(AmountRange.x, AmountRange.y);
            Amount += Mathf.RoundToInt(Amount
                                       * GameManager.Instance.UpgradeBoard.GetUpgradeByName("Gold Bonus").GetValue()
                                       * 0.01f);
        }

        public override void Collected(Hero hero)
        {
            GameManager.Instance.ChangeGoldValue(Amount);
        }

        public override string GetCollectedText()
        {
            return $"+{Amount} gold";
        }
    }
}

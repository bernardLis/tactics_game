using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Arena.Pickup
{
    [CreateAssetMenu(menuName = "ScriptableObject/Arena/Pickups/Coin")]
    public class Coin : Pickup
    {
        public int Amount;

        public override void Initialize()
        {
            // TODO: balance
            Amount = Random.Range(5, 8);
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
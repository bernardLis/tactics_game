using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Casino")]
    public class Casino : Building
    {
        GameManager _gameManager;

        [Header("Casino")]
        public bool IsBetPlaced;
        public bool IsGreenBet;

        public int BetAmount;

        public override void Initialize(Campaign campaign)
        {
            base.Initialize(campaign);
            _gameManager = GameManager.Instance;
        }

        public void PlaceBet(int amount, bool isGreen)
        {
            IsBetPlaced = true;
            IsGreenBet = isGreen;

            if (amount > _gameManager.Gold) amount = _gameManager.Gold;
            _gameManager.ChangeGoldValue(-amount);
            BetAmount = amount;
        }
    }
}
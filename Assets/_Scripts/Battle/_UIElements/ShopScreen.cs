using Lis.Core;
using Lis.Units.Hero.Rewards;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class ShopScreen : RewardScreen
    {
        public ShopScreen()
        {
        }

        protected override void CreateRewardCards()
        {
            // _allRewardElements.Clear();
            // for (int i = 0; i < _numberOfRewards; i++)
            // {
            //     RewardElement el = ChooseRewardElement();
            //     el ??= CreateRewardCardGold(); // backup
            //
            //     el.style.position = Position.Absolute;
            //     float endLeft = _leftPositions[i];
            //     el.style.left = endLeft;
            //
            //     el.style.width = _rewardElementWidth;
            //     el.style.height = _rewardElementHeight;
            //
            //     _rewardContainer.Add(el);
            //     _allRewardElements.Add(el);
            // }


            // create shop element / items that contain
            // reward element and pass it to shop element to price it
            // price


        }

        protected override void RewardSelected(Reward reward)
        {
            // leave empty
        }
    }
}
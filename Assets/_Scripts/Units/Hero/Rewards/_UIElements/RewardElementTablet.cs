using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero.Rewards;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class RewardElementTablet : RewardElement
    {
        public RewardElementTablet(Reward reward) : base(reward)
        {
            RewardTablet rewardTablet = reward as RewardTablet;
            if (rewardTablet == null) return;
            Tablet tablet = rewardTablet.Tablet;

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            Add(container);

            ElementalElement element = new(tablet.Element);
            container.Add(element);

            Label txt = new($"{Helpers.ToRoman(tablet.Level.Value + 1)}");
            txt.style.fontSize = 36;
            txt.style.color = Color.white;
            txt.style.unityFontStyleAndWeight = FontStyle.Bold;
            container.Add(txt);

            TabletElement tabletElement = new(tablet);
            Add(tabletElement);
            tabletElement.style.width = Length.Percent(50);
            tabletElement.style.height = Length.Percent(50);

            Label label = new(Helpers.ParseScriptableObjectName(tablet.name));
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            Add(label);
        }
    }
}
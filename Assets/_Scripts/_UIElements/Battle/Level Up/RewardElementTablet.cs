



using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class RewardElementTablet : RewardElement
    {
        public RewardElementTablet(Reward reward) : base(reward)
        {        
            RewardTablet rewardTablet = reward as RewardTablet;
            Tablet tablet = rewardTablet.Tablet;
            Label txt = new Label("");
            txt.text = $"Level {tablet.Level.Value + 1}";
            Add(txt);

            Add(new TabletElement(tablet, false));

            Label name = new Label(Helpers.ParseScriptableObjectName(tablet.name));
            name.style.whiteSpace = WhiteSpace.Normal;
            name.style.unityFontStyleAndWeight = FontStyle.Bold;
            Add(name);
        }
    }
}

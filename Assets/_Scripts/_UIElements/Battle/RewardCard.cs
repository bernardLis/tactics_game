using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardCard : VisualElement
{

    Reward _reward;
    public RewardCard(Reward reward)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _reward = reward;
        // there probably be a scriptable object with rewards and it should handle 
        // the logic of what reward to give

        // while this is just a place to display the reward

    }
}

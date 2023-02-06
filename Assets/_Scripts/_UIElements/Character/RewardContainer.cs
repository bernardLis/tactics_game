using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using DG.Tweening;

public class RewardContainer : VisualElement
{
    GameManager _gameManager;

    RewardChest _rewardChest;

    AnimationElement _chest;

    Reward _reward;

    int _flyLeft = -1;

    const string _ussClassName = "reward-container__";
    const string _ussMain = _ussClassName + "main";

    public event Action OnChestOpen;
    public RewardContainer(Reward reward, bool clickable = true)
    {
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardContainerStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        _reward = reward;
        _rewardChest = _gameManager.GameDatabase.GetRandomRewardChest();

        _chest = new(_rewardChest.Idle, 100, true);
        _chest.style.width = 200;
        _chest.style.height = 200;
        _chest.PlayAnimation();
        Add(_chest);
        if (clickable)
            _chest.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        _chest.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        RunChestOpenSpectacle();
    }

    void RunChestOpenSpectacle()
    {
        // TODO: I could play some nice VFX here
        AudioManager.Instance.PlaySFX("ChestOpen", Vector3.zero);
        _chest.SwapAnimationSprites(_rewardChest.Open);
        _chest.SetLoop(false);
        _chest.OnAnimationFinished += ShowRewards;

        OnChestOpen?.Invoke();
    }

    void ShowRewards()
    {
        // TODO: there has to be better way to handle rewards
        if (_reward.Gold != 0)
        {
            IVisualElementScheduledItem s = schedule.Execute(FlyGold);
        }

        if (_reward.Item != null)
        {
            IVisualElementScheduledItem s = schedule.Execute(FlyItem).StartingIn(200);
        }

        if (_reward.Spice != 0)
        {
            IVisualElementScheduledItem s = schedule.Execute(FlySpice).StartingIn(400);
        }
    }

    void FlyGold() { FlyingReward(new GoldElement(_reward.Gold)); }

    void FlyItem() { FlyingReward(new ItemElement(_reward.Item)); }

    void FlySpice() { FlyingReward(new SpiceElement(_reward.Spice)); }

    void FlyingReward(VisualElement el)
    {
        Vector3 startPos = new Vector3(Random.Range(0, 50), 100);
        Vector3 endPos = new Vector3(Random.Range(-150, 250) * _flyLeft, Random.Range(100, 200));
        _flyLeft *= -1; // swapping left and right each time it is called

        ArcMovementElement flyingContainer = new(el, startPos, endPos);
        _chest.Add(flyingContainer);
    }
}

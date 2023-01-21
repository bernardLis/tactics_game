using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

public class RewardContainer : VisualElement
{
    GameManager _gameManager;

    RewardChest _rewardChest;
    int _idleSpriteIndex = 0;

    VisualElement _chest;

    Reward _reward;
    IVisualElementScheduledItem _idleAnimation;
    int _flyLeft = -1;
    int _offsetX = 0;

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

        style.width = Length.Percent(100);
        style.flexGrow = 1;
        style.flexShrink = 0;
        style.justifyContent = Justify.Center;

        _reward = reward;
        _rewardChest = GameManager.Instance.GameDatabase.GetRandomRewardChest();

        // first it is hidden in a chest or something that you have to click
        _chest = new();
        _chest.style.width = 200;
        _chest.style.height = 200;

        Add(_chest);
        _idleAnimation = _chest.schedule.Execute(IdleAnimation).Every(100);
        if (clickable)
            _chest.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void IdleAnimation()
    {
        _chest.style.backgroundImage = new StyleBackground(_rewardChest.Idle[_idleSpriteIndex]);
        _idleSpriteIndex++;
        if (_idleSpriteIndex == _rewardChest.Idle.Length)
            _idleSpriteIndex = 0;
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        _chest.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        _idleAnimation.Pause();
        PlayChestOpenAnimation();
    }

    async void PlayChestOpenAnimation()
    {
        // TODO: I could play some nice effect here
        // both sound and fx
        // TODO: there has to be better way to handle reward
        AudioManager.Instance.PlaySFX("ChestOpen", Vector3.zero);
        foreach (Sprite sprite in _rewardChest.Open)
        {
            _chest.style.backgroundImage = new StyleBackground(sprite);
            await Task.Delay(200);
        }

        if (_reward.Gold != 0)
            FlyingReward(new GoldElement(_reward.Gold));

        await Task.Delay(200);

        if (_reward.Item != null)
            FlyingReward(new ItemElement(_reward.Item));

        await Task.Delay(200);

        if (_reward.Spice != 0)
            FlyingReward(new SpiceElement(_reward.Spice));

        await Task.Delay(200);

        OnChestOpen?.Invoke();
    }

    void FlyingReward(VisualElement el)
    {
        VisualElement flyingContainer = new(); // without container the animation transition breaks flying
        flyingContainer.style.visibility = Visibility.Hidden;
        flyingContainer.style.position = Position.Absolute;
        flyingContainer.transform.position = _chest.transform.position;
        Add(flyingContainer);
        flyingContainer.Add(el);

        Vector3 offset = new Vector3(50f * _flyLeft + Random.Range(-50, 50) + _offsetX * _flyLeft, Random.Range(-150, 150));
        Vector3 endPosition = _chest.transform.position + offset;
        _flyLeft *= -1; // swapping left and right each time it is called
        MoveElementOnArc(flyingContainer, _chest.transform.position, endPosition);
    }

    async void MoveElementOnArc(VisualElement el, Vector3 startPosition, Vector3 endPosition)
    {
        el.style.visibility = Visibility.Visible;

        Vector3 p0 = startPosition;
        float newX = startPosition.x + (endPosition.x - startPosition.x) * 0.5f;
        float newY = startPosition.y - 200f;

        Vector3 p1 = new Vector3(newX, newY);
        Vector3 p2 = endPosition;

        float percent = 0;
        while (percent < 1)
        {
            // https://www.reddit.com/r/Unity3D/comments/5pyi43/custom_dotween_easetypeeasefunction_based_on_four/
            Vector3 i1 = Vector3.Lerp(p0, p1, percent); // p1 is the shared handle
            Vector3 i2 = Vector3.Lerp(p1, p2, percent);
            Vector3 result = Vector3.Lerp(i1, i2, percent); // lerp between the 2 for the result

            el.transform.position = result;

            percent += 0.01f;
            await Task.Delay(5);
        }
    }

}

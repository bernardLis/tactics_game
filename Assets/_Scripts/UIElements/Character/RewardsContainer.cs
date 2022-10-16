using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
public class RewardsContainer : VisualElement
{
    ScreenWithDraggables _screenWithDraggables;

    RewardChest _rewardChest;
    int _idleSpriteIndex = 0;

    VisualElement _chest;
    bool _isChestOpened;

    Reward _reward;
    IVisualElementScheduledItem _idleAnimation;
    int _flyLeft = -1;
    int _offsetX = 0;

    public RewardsContainer(Reward reward, ScreenWithDraggables screenWithDraggables)
    {
        style.flexGrow = 1;
        style.flexShrink = 0;
        style.justifyContent = Justify.Center;

        _screenWithDraggables = screenWithDraggables;
        _reward = reward;

        _rewardChest = GameManager.Instance.GameDatabase.GetRandomRewardChest();

        // first it is hidden in a chest or something that you have to click
        _chest = new();
        _chest.style.width = 200;
        _chest.style.height = 200;

        Add(_chest);
        _idleAnimation = _chest.schedule.Execute(IdleAnimation).Every(100);
        _chest.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public bool IsChestOpen()
    {
        if (!_isChestOpened)
            Helpers.DisplayTextOnElement(this, _chest, "Open the chest before going.", Color.red);

        return _isChestOpened;
    }

    void IdleAnimation()
    {
        _chest.style.backgroundImage = new StyleBackground(_rewardChest.Idle[_idleSpriteIndex]);
        _idleSpriteIndex++;
        if (_idleSpriteIndex == 4)
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
        _isChestOpened = true;
        AudioManager.Instance.PlaySFX("ChestOpening", Vector3.zero);
        foreach (Sprite sprite in _rewardChest.Open)
        {
            _chest.style.backgroundImage = new StyleBackground(sprite);
            await Task.Delay(200);
        }

        if (_reward.Gold != 0)
            FlyingReward(new GoldElement(_reward.Gold, true));

        await Task.Delay(200);

        if (_reward.Item != null)
            FlyingReward(_screenWithDraggables.CreateDraggableItem(_reward.Item));
    }

    void FlyingReward(VisualElement el)
    {
        VisualElement flyingContainer = new(); // without container the animation transition breaks flying
        flyingContainer.style.visibility = Visibility.Hidden;
        flyingContainer.style.position = Position.Absolute;
        flyingContainer.transform.position = _chest.transform.position;
        Add(flyingContainer);
        flyingContainer.Add(el);

        Vector3 offset = new Vector3(250f * _flyLeft + Random.Range(-50, 50) + _offsetX * _flyLeft, Random.Range(-10, 10));
        Vector3 endPosition = _chest.transform.position + offset;
        _flyLeft *= -1; // swapping left and right each time it is called
        if (_flyLeft == 1)
            _offsetX += 100; // TODO: throw every "2nd" element a bit further so they don't interfere
        MoveElementOnArc(flyingContainer, _chest.transform.position, endPosition);
    }

    async void MoveElementOnArc(VisualElement el, Vector3 startPosition, Vector3 endPositon)
    {
        el.style.visibility = Visibility.Visible;

        Vector3 p0 = startPosition;
        float newX = startPosition.x + (endPositon.x - startPosition.x) * 0.5f;
        float newY = startPosition.y - 200f;

        Vector3 p1 = new Vector3(newX, newY);
        Vector3 p2 = endPositon;

        float percent = 0;
        while (percent < 1)
        {
            // https://www.reddit.com/r/Unity3D/comments/5pyi43/custom_dotween_easetypeeasefunction_based_on_four/
            Vector3 i1 = Vector3.Lerp(p0, p1, percent); // p1 is the shared handle
            Vector3 i2 = Vector3.Lerp(p1, p2, percent);
            Vector3 result = Vector3.Lerp(i1, i2, percent); // lerp between the 2 for the result

            //Vector3 pointOnArc = DOCurve.CubicBezier.GetPointOnSegment(startPosition, startPosition, endPositon, endPositon, percent);
            el.transform.position = result;

            percent += 0.01f;
            await Task.Delay(5);
        }
    }

}

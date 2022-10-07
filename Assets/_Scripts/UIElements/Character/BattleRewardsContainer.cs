using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class BattleRewardsContainer : VisualElement
{

    BattleEndScreen _screenWithDraggables;

    Sprite[] _chestIdleSprites;
    int _spriteIndex = 0;

    Sprite[] _chestOpenSprites;

    VisualElement _chest;
    bool _isChestOpened;

    JourneyNodeReward _journeyNodeReward;
    IVisualElementScheduledItem _idleAnimation;
    int _flyLeft = -1;

    public BattleRewardsContainer(JourneyNodeReward journeyNodeReward, BattleEndScreen screenWithDraggables)
    {
        style.flexGrow = 1;
        style.flexShrink = 0;
        style.justifyContent = Justify.Center;

        _screenWithDraggables = screenWithDraggables;
        _journeyNodeReward = journeyNodeReward;

        _chestIdleSprites = GameManager.Instance.GameDatabase.RewardChestIdle;
        _chestOpenSprites = GameManager.Instance.GameDatabase.RewardChestOpen;

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
        _chest.style.backgroundImage = new StyleBackground(_chestIdleSprites[_spriteIndex]);
        _spriteIndex++;
        if (_spriteIndex == 4)
            _spriteIndex = 0;
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

        foreach (Sprite sprite in _chestOpenSprites)
        {
            _chest.style.backgroundImage = new StyleBackground(sprite);
            await Task.Delay(100);
        }

        if (_journeyNodeReward.Gold != 0)
            FlyingReward(new Label($"Gold {_journeyNodeReward.Gold}"));

        if (_journeyNodeReward.Item != null)
            FlyingReward(CreateDraggableItem(_journeyNodeReward.Item));

    }

    VisualElement CreateDraggableItem(Item item)
    {
        ItemSlotVisual slot = new ItemSlotVisual();
        ItemVisual itemVisual = new(item);
        slot.AddItem(itemVisual);
        _screenWithDraggables.AddNewDraggableItem(slot);

        return slot;
    }

    void FlyingReward(VisualElement el)
    {
        Debug.Log($"flying reward {el}");
        Add(el);
        el.AddToClassList("textPrimary");
        // TODO: https://docs.unity3d.com/Manual/UIE-Transform.html
        // maybe i can reposition them elements with https://gamedev-resources.com/use-style-transitions-to-animate-a-menu/?utm_source=mailpoet&utm_medium=email&utm_campaign=new-tutorial-create-a-map-system
        el.style.position = Position.Absolute;
        el.transform.position = _chest.transform.position;
        float endx = _chest.transform.position.x + 150f;
        float endy = _chest.transform.position.y - 150f;

        Vector3 offset = new Vector3(250f * _flyLeft + Random.Range(-50, 50), Random.Range(-10, 10));
        Vector3 endPosition = _chest.transform.position + offset;
        _flyLeft *= -1; // swapping left and right each time it is called
        MoveElementOnArc(el, _chest.transform.position, endPosition);
        //el.style.left = _chest.layout.xMin;
        //float topEnd = _chest.layout.yMin + 100;
        //DOTween.To(x => el.transform.position = x * Vector3.one, endposition, 1f);
        // TODO: arc movement
        // http://forum.demigiant.com/index.php?topic=26.0
        //DOTween.To(() => el.transform.position, x => el.transform.position = x, endPosition, 1f).SetEase(Ease.InOutQuad);
    }

    async void MoveElementOnArc(VisualElement el, Vector3 startPosition, Vector3 endPositon)
    {
        Vector3 p0 = startPosition;
        float newX = startPosition.x + (endPositon.x - startPosition.x) * 0.5f;
        float newY = startPosition.y - 200f;

        Vector3 p1 = new Vector3(newX, newY);
        Debug.Log($"p1 {p1}");
        Vector3 p2 = endPositon;

        Debug.Log($"moving element on arc");
        float percent = 0;
        while (percent < 1)
        {
            // https://www.reddit.com/r/Unity3D/comments/5pyi43/custom_dotween_easetypeeasefunction_based_on_four/
            Vector3 i1 = Vector3.Lerp(p0, p1, percent); // p1 is the shared handle
            Vector3 i2 = Vector3.Lerp(p1, p2, percent);
            Vector3 result = Vector3.Lerp(i1, i2, percent); // lerp between the 2 for the result

            Debug.Log($"point on arc {result} , time: {percent}");

            //Vector3 pointOnArc = DOCurve.CubicBezier.GetPointOnSegment(startPosition, startPosition, endPositon, endPositon, percent);
            el.transform.position = result;
            percent += 0.01f;
            await Task.Delay(5);
        }
    }

}

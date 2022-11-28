using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationVisualElement : VisualElement
{
    Sprite[] _animationSprites;
    bool _isLoop;
    IVisualElementScheduledItem _animationScheduler;
    int _animationSpriteIndex = 0;

    public bool IsAnimationFinished { get; private set; }

    public AnimationVisualElement(Sprite[] animationSprites, int delay, bool isLoop)
    {
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);

        _animationSprites = animationSprites;
        _isLoop = isLoop;
        _animationScheduler = schedule.Execute(Animation).Every(delay);
    }

    public void SwapAnimationSprites(Sprite[] animationSprites)
    {
        _animationSprites = animationSprites;
        _animationSpriteIndex = 0;
    }

    void Animation()
    {
        style.backgroundImage = new StyleBackground(_animationSprites[_animationSpriteIndex]);
        _animationSpriteIndex++;
        if (_animationSpriteIndex == _animationSprites.Length)
        {
            if (_isLoop)
                _animationSpriteIndex = 0;
            else
                FinishAnimation();
        }
    }

    void FinishAnimation()
    {
        _animationScheduler.Pause();
        IsAnimationFinished = true;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationElement : VisualElement
{
    Sprite[] _animationSprites;
    bool _isLoop;
    int _delay;
    int _animationSpriteIndex = 0;

    IVisualElementScheduledItem _animationScheduler;

    bool _isFinished;

    public event Action OnAnimationFinished;
    public AnimationElement(Sprite[] animationSprites, int delay, bool isLoop)
    {
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        _animationSprites = animationSprites;
        _delay = delay;
        _isLoop = isLoop;
    }

    public void SwapAnimationSprites(Sprite[] animationSprites)
    {
        _animationSprites = animationSprites;
        _animationSpriteIndex = 0;
    }

    public void SetLoop(bool isLoop) { _isLoop = isLoop; }
    public bool IsAnimationFinished() { return _isFinished; }
    public void PlayAnimation() { _animationScheduler = schedule.Execute(Animate).Every(_delay); }
    public void PauseAnimation() { _animationScheduler.Pause(); }
    public void ResumeAnimation() { _animationScheduler.Resume(); }

    void Animate()
    {
        if (_animationSpriteIndex == _animationSprites.Length)
        {
            if (!_isLoop)
            {
                FinishAnimation();
                return;
            }
            _animationSpriteIndex = 0;
        }
        style.backgroundImage = new StyleBackground(_animationSprites[_animationSpriteIndex]);
        _animationSpriteIndex++;
    }

    void FinishAnimation()
    {
        PauseAnimation();
        OnAnimationFinished?.Invoke();
    }
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class AnimationElement : VisualElement
    {
        private readonly int _delay;

        private IVisualElementScheduledItem _animationScheduler;
        private int _animationSpriteIndex;
        private Sprite[] _animationSprites;

        private bool _isFinished;
        private bool _isLoop;

        public AnimationElement(Sprite[] animationSprites, int delayBetweenSprites, bool isLoop, bool noStyles = false)
        {
            if (!noStyles)
            {
                style.width = Length.Percent(100);
                style.height = Length.Percent(100);
                style.position = Position.Absolute;
            }

            _animationSprites = animationSprites;
            _delay = delayBetweenSprites;
            _isLoop = isLoop;

            style.backgroundImage = new(_animationSprites[0]);
            style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        }

        public event Action OnAnimationFinished;

        public void SwapAnimationSprites(Sprite[] animationSprites)
        {
            _animationSprites = animationSprites;
            _animationSpriteIndex = 0;
            style.backgroundImage = new(_animationSprites[0]);
        }

        public void SetLoop(bool isLoop)
        {
            _isLoop = isLoop;
        }

        public bool IsAnimationFinished()
        {
            return _isFinished;
        }

        public void PlayAnimation()
        {
            if (_animationSprites.Length <= 1) return;

            _animationScheduler = schedule.Execute(Animate).Every(_delay);
        }

        public void PauseAnimation()
        {
            if (_animationScheduler != null)
                _animationScheduler.Pause();
        }

        public void ResumeAnimation()
        {
            _animationScheduler.Resume();
        }

        private void Animate()
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

            style.backgroundImage = new(_animationSprites[_animationSpriteIndex]);
            _animationSpriteIndex++;
        }

        private void FinishAnimation()
        {
            PauseAnimation();
            OnAnimationFinished?.Invoke();
        }
    }
}
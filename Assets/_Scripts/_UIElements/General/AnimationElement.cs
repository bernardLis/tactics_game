using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class AnimationElement : VisualElement
{
    Sprite[] _animationSprites;
    bool _isLoop;
    int _delay;
    int _animationSpriteIndex = 0;

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

    public async void PlayAnimation() { await AwaitablePlayAnimation(); }

    public async Task AwaitablePlayAnimation()
    {
        while (_animationSpriteIndex <= _animationSprites.Length)
        {
            if (_animationSpriteIndex == _animationSprites.Length)
                if (_isLoop)
                    _animationSpriteIndex = 0;
                else
                    return;
            style.backgroundImage = new StyleBackground(_animationSprites[_animationSpriteIndex]);
            _animationSpriteIndex++;

            await Task.Delay(_delay);
        }
    }

}
